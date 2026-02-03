using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IRichTextDocument interface.
    /// Represents text in RTF format. Obtain from ITextObject.GetRichTextDocument() or GetInterfaceByName("IRichTextDocument").
    /// </summary>
    public class IRichTextDocument : IDisposable
    {
        private Renga.IRichTextDocument _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object Renga.IRichTextDocument
        /// </summary>
        public Renga.IRichTextDocument _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates IRichTextDocument from COM object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextDocument(object richTextDocument)
        {
            if (richTextDocument == null)
                throw new ArgumentNullException(nameof(richTextDocument), "IRichTextDocument cannot be null");
            _i = richTextDocument as Renga.IRichTextDocument;
            if (_i == null)
                throw new ArgumentException("Object does not implement IRichTextDocument interface", nameof(richTextDocument));
        }

        /// <summary>
        /// Constructor - Creates IRichTextDocument from typed COM object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextDocument(Renga.IRichTextDocument richTextDocument)
        {
            _i = richTextDocument ?? throw new ArgumentNullException(nameof(richTextDocument));
        }

        ~IRichTextDocument()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_i != null)
                {
                    Marshal.ReleaseComObject(_i);
                    _i = null;
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets the number of paragraphs in the document
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int ParagraphCount
        {
            get
            {
                if (_i == null)
                    throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
                try
                {
                    return _i.ParagraphCount;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get paragraph count: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Returns the paragraph at the given index (zero-based)
        /// </summary>
        /// <param name="index">Paragraph index</param>
        /// <returns>IRichTextParagraph wrapper or null</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph GetParagraph(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            try
            {
                var para = _i.GetParagraph(index);
                return para != null ? new IRichTextParagraph(para) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get paragraph at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes the paragraph at the given index
        /// </summary>
        /// <param name="index">Paragraph index (zero-based)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void RemoveParagraph(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            try
            {
                _i.RemoveParagraph(index);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove paragraph at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appends a paragraph to the end. Data must contain at least one token.
        /// See https://help.rengabim.com/api/interface_i_rich_text_document.html
        /// </summary>
        /// <param name="tokens">List of RichTextToken (at least one). Create with RichTextToken.ByTextAndFont or from GetToken.</param>
        /// <returns>The new IRichTextParagraph</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph AppendParagraph(List<RichTextToken> tokens)
        {
            const string logPath = @"d:\REP\RengaDyn\.cursor\debug.log";
            void Log(string loc, string msg, string dataJson, string hid)
            {
                try
                {
                    var sb = new StringBuilder();
                    sb.Append("{\"timestamp\":").Append(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                    sb.Append(",\"location\":\"").Append(loc.Replace("\\", "\\\\").Replace("\"", "\\\""));
                    sb.Append("\",\"message\":\"").Append((msg ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\r", "").Replace("\n", " "));
                    sb.Append("\",\"data\":").Append(dataJson ?? "{}");
                    sb.Append(",\"hypothesisId\":\"").Append(hid ?? "").Append("\"}\n");
                    File.AppendAllText(logPath, sb.ToString());
                }
                catch { }
            }
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("At least one token is required.", nameof(tokens));
            int paragraphCountBefore = -1;
            try { paragraphCountBefore = _i.ParagraphCount; } catch { }
            // #region agent log
            Log("IRichTextDocument.AppendParagraph:entry", "AppendParagraph called", "{\"paragraphCountBefore\":" + paragraphCountBefore + ",\"tokensCount\":" + tokens.Count + "}", "H4,H5");
            // #endregion
            try
            {
                // #region agent log
                Log("IRichTextDocument.AppendParagraph:beforeToArray", "Before ToRenga/ToArray", "{\"tokensCount\":" + tokens.Count + "}", "H2");
                // #endregion
                var data = tokens.Select(t => t.ToRenga()).ToArray();
                // #region agent log
                Log("IRichTextDocument.AppendParagraph:afterToArray", "Before COM AppendParagraph", "{\"dataLength\":" + data.Length + "}", "H1");
                // #endregion
                var para = _i.AppendParagraph(data);
                // #region agent log
                Log("IRichTextDocument.AppendParagraph:afterCall", "COM returned", "{\"paraIsNull\":" + (para == null ? "true" : "false") + "}", "H3,H4");
                // #endregion
                var result = para != null ? new IRichTextParagraph(para) : null;
                // #region agent log
                Log("IRichTextDocument.AppendParagraph:beforeReturn", "Before return wrapper", "{\"resultIsNull\":" + (result == null ? "true" : "false") + "}", "H3");
                // #endregion
                return result;
            }
            catch (Exception ex)
            {
                var stack = ex.StackTrace ?? ""; if (stack.Length > 400) stack = stack.Substring(0, 400);
                var esc = new Func<string, string>(s => (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\""));
                // #region agent log
                Log("IRichTextDocument.AppendParagraph:catch", "Exception", "{\"exMessage\":\"" + esc(ex.Message) + "\",\"exHResult\":" + ex.HResult + ",\"innerHResult\":" + (ex.InnerException?.HResult ?? 0) + ",\"exType\":\"" + esc(ex.GetType().FullName) + "\",\"stack\":\"" + esc(stack) + "\"}", "H1,H2,H3");
                // #endregion
                throw new InvalidOperationException($"Failed to append paragraph: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appends a paragraph inside an operation (transaction). Use this when AppendParagraph returns null —
        /// Renga may require an active operation to create paragraphs. Call operation.Start() before, then this method, then operation.Apply().
        /// </summary>
        /// <param name="tokens">List of RichTextToken (at least one). Create with RichTextToken.ByTextAndFont or from GetToken.</param>
        /// <param name="operation">IOperation from IProject.CreateOperation() or IModel.CreateOperation(). Will Start() then AppendParagraph then Apply().</param>
        /// <returns>The new IRichTextParagraph, or null if COM still returns null.</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph AppendParagraph(List<RichTextToken> tokens, IOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation), "IOperation is required for AppendParagraph with operation.");
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("At least one token is required.", nameof(tokens));
            operation.Start();
            try
            {
                var data = tokens.Select(t => t.ToRenga()).ToArray();
                var para = _i.AppendParagraph(data);
                operation.Apply();
                return para != null ? new IRichTextParagraph(para) : null;
            }
            catch
            {
                try { operation.Rollback(); } catch { }
                throw;
            }
        }

        /// <summary>
        /// Inserts a paragraph at the given index. Data must contain at least one token.
        /// See https://help.rengabim.com/api/interface_i_rich_text_document.html
        /// </summary>
        /// <param name="index">Paragraph index (zero-based)</param>
        /// <param name="tokens">List of RichTextToken (at least one)</param>
        /// <returns>The new IRichTextParagraph</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph InsertParagraph(int index, List<RichTextToken> tokens)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("At least one token is required.", nameof(tokens));
            try
            {
                var data = tokens.Select(t => t.ToRenga()).ToArray();
                var para = _i.InsertParagraph(index, data);
                return para != null ? new IRichTextParagraph(para) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to insert paragraph at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Prepends a paragraph at the beginning. Data must contain at least one token.
        /// See https://help.rengabim.com/api/interface_i_rich_text_document.html
        /// </summary>
        /// <param name="tokens">List of RichTextToken (at least one)</param>
        /// <returns>The new IRichTextParagraph</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph PrependParagraph(List<RichTextToken> tokens)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextDocument interface is not initialized.");
            if (tokens == null || tokens.Count == 0)
                throw new ArgumentException("At least one token is required.", nameof(tokens));
            try
            {
                var data = tokens.Select(t => t.ToRenga()).ToArray();
                var para = _i.PrependParagraph(data);
                return para != null ? new IRichTextParagraph(para) : null;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to prepend paragraph: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}
