// Control module UI logic using webform.js messaging

const MODE_MANUAL = 'manual';
const MODE_AUTOMATIC = 'automatic';

let currentMode = MODE_AUTOMATIC;
let initialPreviewDone = false;
let previewDebounce;
const debounceMs = 120;
let lastPreviewPayload = null;
let lastAppliedRC = { rows: null, cols: null };

function ensureCanvasSize(canvas){
  const rect = canvas.getBoundingClientRect();
  const dpr = window.devicePixelRatio || 1;
  const cssW = Math.max(1, Math.floor(rect.width));
  const cssH = Math.max(1, Math.floor(rect.height));
  const pxW = Math.max(1, Math.floor(cssW * dpr));
  const pxH = Math.max(1, Math.floor(cssH * dpr));
  if (canvas.width !== pxW || canvas.height !== pxH) {
    canvas.width = pxW;
    canvas.height = pxH;
  }
  return { cssW, cssH, dpr };
}

function getParams(){
  const rowsInput = document.getElementById('rows');
  const colsInput = document.getElementById('cols');
  const rows = Math.max(1, parseInt(rowsInput?.value ?? '1', 10) || 1);
  const cols = Math.max(1, parseInt(colsInput?.value ?? '1', 10) || 1);

  const base = {
    rows,
    cols,
    wallGap: parseFloat(document.getElementById('wallGap')?.value ?? '0') || 0,
    gap: parseFloat(document.getElementById('gap')?.value ?? '0') || 0,
    alignTol: parseFloat(document.getElementById('alignTol')?.value ?? '0') || 0,
    vOff: parseFloat(document.getElementById('vOff')?.value ?? '0') || 0,
    round: parseFloat(document.getElementById('round')?.value ?? '0') || 0,
    rot: parseFloat(document.getElementById('rot')?.value ?? '0') || 0,
    materialId: parseInt((document.getElementById('material')?.value || '0'), 10) || 0
  };

  base.lightFlood = currentMode === MODE_AUTOMATIC;
  base.maskHoles = currentMode === MODE_MANUAL && !!document.getElementById('maskHoles')?.checked;

  return base;
}

function populateMaterials(list){
  const sel = document.getElementById('material');
  if (!sel) return;

  const previous = sel.dataset.selected || sel.value;
  sel.innerHTML = '';

  let matched = false;
  (list || []).forEach(item => {
    const opt = document.createElement('option');
    opt.value = item.id;
    opt.textContent = `${item.id}: ${item.name}`;
    if (!matched && String(item.id) === previous) {
      opt.selected = true;
      matched = true;
    }
    sel.appendChild(opt);
  });

  if (!matched && sel.options.length > 0) {
    sel.selectedIndex = 0;
  }

  sel.dataset.selected = sel.value;
}

function requestData(){
  sendMessage('PageLoaded', {});
}

function drawPreview(payload){
  if (payload) lastPreviewPayload = payload;
  const canvas = document.getElementById('preview');
  if (!canvas) return;
  const ctx = canvas.getContext('2d');
  const { cssW, cssH, dpr } = ensureCanvasSize(canvas);
  ctx.setTransform(dpr, 0, 0, dpr, 0, 0);
  ctx.clearRect(0, 0, cssW, cssH);
  const p = lastPreviewPayload;
  if (!p || !p.room) {
    ctx.fillStyle = '#5a5a6c';
    ctx.font = '12px "Inter", system-ui';
    ctx.fillText('Нет данных', 20, 24);
    return;
  }

  const roomInfo = document.getElementById('roomInfo');
  if (roomInfo) roomInfo.textContent = `Помещение: ${p.room.name} • ${Math.round(p.room.width)} x ${Math.round(p.room.height)} мм`;

  const pad = 18;
  const sx = (cssW - pad * 2) / p.room.width;
  const sy = (cssH - pad * 2) / p.room.height;
  const scale = Math.min(sx, sy);
  const ox = (cssW - p.room.width * scale) / 2;
  const oy = (cssH - p.room.height * scale) / 2;
  const toXY = pt => ({
    x: ox + (pt.x - p.room.minX) * scale,
    y: oy + (p.room.maxY - pt.y) * scale
  });

  ctx.lineWidth = 1.1;
  ctx.lineJoin = 'round';
  ctx.lineCap = 'round';
  const roundMm = Math.max(0, p?.round ?? 0);

  function poly(points, fill, stroke, localRoundMm = 0){
    if (!points || points.length < 3) return;
    const pxPts = [];
    const firstPt = points[0];
    const hasClosing = points.length > 1 && Math.abs(points[points.length - 1].x - firstPt.x) < 1e-6 && Math.abs(points[points.length - 1].y - firstPt.y) < 1e-6;
    const limit = hasClosing ? points.length - 1 : points.length;
    for (let i = 0; i < limit; i++) {
      const pt = points[i];
      if (!pt) continue;
      pxPts.push(toXY(pt));
    }
    if (pxPts.length < 3) return;

    let radiusPx = Math.max(0, localRoundMm * scale);
    if (radiusPx > 0) {
      let maxRadius = Number.POSITIVE_INFINITY;
      for (let i = 0; i < pxPts.length; i++) {
        const prev = pxPts[(i + pxPts.length - 1) % pxPts.length];
        const curr = pxPts[i];
        const next = pxPts[(i + 1) % pxPts.length];
        const lenPrev = Math.hypot(curr.x - prev.x, curr.y - prev.y);
        const lenNext = Math.hypot(next.x - curr.x, next.y - curr.y);
        if (lenPrev < 1e-3 || lenNext < 1e-3) { maxRadius = 0; break; }
        maxRadius = Math.min(maxRadius, lenPrev * 0.5, lenNext * 0.5);
      }
      radiusPx = Math.min(radiusPx, maxRadius);
      if (!isFinite(radiusPx) || radiusPx < 0.3) radiusPx = 0;
    }

    ctx.beginPath();
    if (radiusPx > 0) {
      let startPoint = null;
      for (let i = 0; i < pxPts.length; i++) {
        const prev = pxPts[(i + pxPts.length - 1) % pxPts.length];
        const curr = pxPts[i];
        const next = pxPts[(i + 1) % pxPts.length];
        const vx1 = curr.x - prev.x;
        const vy1 = curr.y - prev.y;
        const vx2 = next.x - curr.x;
        const vy2 = next.y - curr.y;
        const len1 = Math.hypot(vx1, vy1);
        const len2 = Math.hypot(vx2, vy2);
        if (len1 < 1e-3 || len2 < 1e-3) {
          if (i === 0) { ctx.moveTo(curr.x, curr.y); startPoint = { x: curr.x, y: curr.y }; }
          else ctx.lineTo(curr.x, curr.y);
          continue;
        }
        const ux1 = vx1 / len1; const uy1 = vy1 / len1;
        const ux2 = vx2 / len2; const uy2 = vy2 / len2;
        let dot = ux1 * ux2 + uy1 * uy2;
        dot = Math.max(-0.9999, Math.min(0.9999, dot));
        const angle = Math.acos(dot);
        const tanHalf = Math.tan(angle / 2);
        let inset = tanHalf > 1e-6 ? radiusPx / tanHalf : radiusPx;
        inset = Math.min(inset, len1 / 2, len2 / 2);
        const p1x = curr.x - ux1 * inset;
        const p1y = curr.y - uy1 * inset;
        const p2x = curr.x + ux2 * inset;
        const p2y = curr.y + uy2 * inset;
        if (i === 0) {
          ctx.moveTo(p1x, p1y);
          startPoint = { x: p1x, y: p1y };
        } else {
          ctx.lineTo(p1x, p1y);
        }
        ctx.quadraticCurveTo(curr.x, curr.y, p2x, p2y);
      }
      if (startPoint) ctx.lineTo(startPoint.x, startPoint.y);
      ctx.closePath();
    } else {
      ctx.moveTo(pxPts[0].x, pxPts[0].y);
      for (let i = 1; i < pxPts.length; i++) ctx.lineTo(pxPts[i].x, pxPts[i].y);
      ctx.closePath();
    }

    if (fill) { ctx.fillStyle = fill; ctx.fill(); }
    if (stroke) { ctx.strokeStyle = stroke; ctx.stroke(); }
  }

  ctx.strokeStyle = '#323246';
  // ctx.strokeRect(ox, oy, p.room.width * scale, p.room.height * scale);
  ctx.fillStyle = '#74748d';
  ctx.font = '12px "Inter", system-ui';
  // ctx.fillText(`W:${Math.round(p.room.width)} H:${Math.round(p.room.height)}`, ox + 6, oy + 16);

  poly(p.polygon, 'rgba(120, 86, 255, 0.18)', '#6e54ff');
  if (p.inset) poly(p.inset, null, '#3a3a52');

  if (Array.isArray(p.columns)) {
    p.columns.forEach(columnPoly => poly(columnPoly, 'rgba(255, 140, 0, 0.08)', '#f08c00'));
  }

  if (Array.isArray(p.cells)) {
    p.cells.forEach(cellEntry => {
      if (!cellEntry) return;
      if (Array.isArray(cellEntry)) {
        poly(cellEntry, 'rgba(70, 160, 255, 0.28)', '#47a0ff', roundMm);
        return;
      }
      const outline = Array.isArray(cellEntry.outline) ? cellEntry.outline : null;
      if (outline) poly(outline, 'rgba(70, 160, 255, 0.28)', '#47a0ff', roundMm);
      if (Array.isArray(cellEntry.holes)) {
        cellEntry.holes.forEach(hole => {
          if (!Array.isArray(hole)) return;
          poly(hole, 'rgba(27, 27, 40, 0.75)', '#1b1b28', roundMm);
        });
      }
    });
  }
  if (p.lightFlood && Array.isArray(p.lights)) p.lights.forEach(light => {
    const q = toXY(light);
    ctx.fillStyle = '#ff9580';
    ctx.beginPath();
    ctx.arc(q.x, q.y, 4, 0, Math.PI * 2);
    ctx.fill();
  });
}

function attachResizeHandlers(){
  const canvas = document.getElementById('preview');
  if (!canvas) return;
  window.addEventListener('resize', () => { if (lastPreviewPayload) drawPreview(); });
  if (typeof ResizeObserver === 'function') {
    const ro = new ResizeObserver(() => { if (lastPreviewPayload) drawPreview(); });
    ro.observe(canvas);
  }
}

function updateModeUI(){
  document.body.classList.toggle('manual-mode', currentMode === MODE_MANUAL);
  document.body.classList.toggle('automatic-mode', currentMode === MODE_AUTOMATIC);

  document.querySelectorAll('[data-mode-toggle]').forEach(btn => {
    btn.classList.toggle('active', btn.dataset.modeToggle === currentMode);
  });

  const rowsInput = document.getElementById('rows');
  const colsInput = document.getElementById('cols');
  const maskHoles = document.getElementById('maskHoles');
  const calcBtn = document.getElementById('btnCalcGrid');
  const vOff = document.getElementById('vOff');

  const manual = currentMode === MODE_MANUAL;
  if (rowsInput) rowsInput.disabled = !manual;
  if (colsInput) colsInput.disabled = !manual;
  if (maskHoles) maskHoles.disabled = !manual;
  if (calcBtn) calcBtn.disabled = !manual;
  if (vOff) vOff.disabled = !manual;
}

function setMode(mode, { silent = false } = {}){
  if (mode !== MODE_MANUAL && mode !== MODE_AUTOMATIC) return;
  if (currentMode === mode) return;
  currentMode = mode;
  updateModeUI();
  if (!silent) sendMessage('Preview', getParams());
}

function initModeSwitch(){
  document.querySelectorAll('[data-mode-toggle]').forEach(btn => {
    btn.addEventListener('click', () => setMode(btn.dataset.modeToggle));
  });
  updateModeUI();
}

function initPanels(){
  document.querySelectorAll('.panel').forEach(panel => {
    if (!panel.hasAttribute('aria-expanded')) panel.setAttribute('aria-expanded', 'true');
  });

  document.querySelectorAll('[data-collapse]').forEach(btn => {
    btn.addEventListener('click', () => {
      const panel = btn.closest('.panel');
      if (!panel) return;
      const expanded = panel.getAttribute('aria-expanded') !== 'false';
      panel.setAttribute('aria-expanded', expanded ? 'false' : 'true');
    });
  });
}

function initNavGroups(){
  const navChips = document.querySelectorAll('[data-nav]');
  navChips.forEach(chip => {
    chip.addEventListener('click', () => {
      navChips.forEach(btn => btn.classList.toggle('active', btn === chip));
    });
  });

  const typeChips = document.querySelectorAll('[data-type]');
  typeChips.forEach(chip => {
    chip.addEventListener('click', () => {
      typeChips.forEach(btn => btn.classList.toggle('active', btn === chip));
    });
  });
}

function initEvents(){
  document.getElementById('btnCreate')?.addEventListener('click', () => sendMessage('Create', getParams()));
  document.getElementById('btnPreview')?.addEventListener('click', () => sendMessage('Preview', getParams()));
  document.getElementById('btnPreviewToolbar')?.addEventListener('click', () => sendMessage('Preview', getParams()));
  document.getElementById('btnUndo')?.addEventListener('click', () => sendMessage('Undo', {}));
  document.getElementById('btnCalcGrid')?.addEventListener('click', () => sendMessage('CalcGridFromLights', getParams()));

  const materialSelect = document.getElementById('material');
  if (materialSelect) materialSelect.addEventListener('change', () => { materialSelect.dataset.selected = materialSelect.value; });
}

// Chain the shared webform.js handler so spinner/toasts continue to work
const __baseWebFormDataUpdate = window.webFormDataUpdate;
function webFormDataUpdate(event){
  try { console.log('control:webmsg', event?.data?.Name, event?.data); } catch (_) {}

  if (event?.data?.Name === 'Status' && event?.data?.Data) {
    try { showAlert(event.data.Data.header, event.data.Data.message, event.data.Data.status, event.data.Data.autohide); } catch (_) {}
    try { hideSpinner(); } catch (_) {}
    return;
  }

  try { if (typeof __baseWebFormDataUpdate === 'function') __baseWebFormDataUpdate(event); } catch (e) { /* ignore */ }

  if (event.data.Name === 'Materials') {
    populateMaterials(event.data.Data);
    if (!initialPreviewDone) { initialPreviewDone = true; sendMessage('Preview', getParams()); }
  }

  if (event.data.Name === 'PreviewData') drawPreview(event.data.Data);

  if (event.data.Name === 'UpdateParams' && currentMode === MODE_MANUAL) {
    try {
      const data = event.data.Data || {};
      let need = false;
      if (typeof data.rows === 'number') {
        const cur = parseInt(document.getElementById('rows').value, 10);
        if (cur !== data.rows) { document.getElementById('rows').value = String(data.rows); need = true; }
      }
      if (typeof data.cols === 'number') {
        const cur = parseInt(document.getElementById('cols').value, 10);
        if (cur !== data.cols) { document.getElementById('cols').value = String(data.cols); need = true; }
      }
      if (need) {
        lastAppliedRC = {
          rows: parseInt(document.getElementById('rows').value, 10),
          cols: parseInt(document.getElementById('cols').value, 10)
        };
        clearTimeout(previewDebounce);
        previewDebounce = setTimeout(() => sendMessage('Preview', getParams()), debounceMs);
      }
    } catch (_) {}
  }

  if (event.data.Name === 'JobDone') { try { hideSpinner(); } catch (_) {} }
}

window.addEventListener('DOMContentLoaded', () => {
  initPanels();
  initNavGroups();
  initModeSwitch();
  initEvents();
  attachResizeHandlers();
  requestData();
});
