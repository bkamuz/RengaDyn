using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Aw.Interface;
using NLog;
using Renga;

namespace Aw.Plugin.Control
{
    /// <summary>
    /// Entry point for the Control plugin. Delegates most logic to <see cref="RoomPerimeterManager"/>.
    /// </summary>
    public class Handling : IRengaPlugin
    {
        private static Handling _instance;
        public static Handling GetInstance() => _instance;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly RoomPerimeterManager _manager;

        public List<RengaAction> Actions { get; set; }
        public byte MenuId { get; } = 1;

        public Handling(object parameter)
        {
            _instance = this;

            try
            {
                var application = parameter as IApplication ?? throw new InvalidOperationException("Renga application is not available");
                var ui = application.UI;

                _manager = new RoomPerimeterManager(application, ui);
                _manager.Initialize();

                ActionsCreate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to initialise Control plugin");
            }
        }

        public void Dispose()
        {
            try
            {
                _manager?.Stop();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Control plugin manager stop failed");
            }

            var form = ControlWeb.Instance;
            if (form != null && !form.IsDisposed)
            {
                form.WindowState = FormWindowState.Normal;
                form.Close();
            }
        }

        private void ActionsCreate()
        {
            Actions = new List<RengaAction>();

            ActionCreate(
                title: "Генератор контролов",
                menu: ActionRengaMenu.MainMenu,
                showCase: ActionRengaShowCase.None,
                viewType: ActionRengaViewType.None,
                iconFileName: "\\Control.png",
                handler: ShowControl,
                orderBy: 200);
        }

        private void ActionCreate(
            string title,
            ActionRengaMenu menu,
            ActionRengaShowCase showCase,
            ActionRengaViewType viewType,
            string iconFileName,
            RengaAction.ActionHandler handler,
            int orderBy = 999)
        {
            var action = new RengaAction(title, menu, showCase, viewType, iconFileName, handler, orderBy);
            Actions.Add(action);
        }

        private void ShowControl(object _)
        {
            try
            {
                var form = ControlWeb.GetInstance(_manager);
                form.Show();
                form.Activate();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to open Control UI");
            }
        }
    }
}
