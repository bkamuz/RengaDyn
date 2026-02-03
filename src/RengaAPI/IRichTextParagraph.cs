using System;
using System.Runtime.InteropServices;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga IRichTextParagraph interface.
    /// Represents a text paragraph (structural element of IRichTextDocument). Obtain from IRichTextDocument.GetParagraph(index).
    /// </summary>
    public class IRichTextParagraph : IDisposable
    {
        private Renga.IRichTextParagraph _i;
        private bool _disposed = false;

        /// <summary>
        /// Internal COM object Renga.IRichTextParagraph
        /// </summary>
        public Renga.IRichTextParagraph _i_Internal => _i;

        /// <summary>
        /// Constructor - Creates IRichTextParagraph from COM object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph(object richTextParagraph)
        {
            if (richTextParagraph == null)
                throw new ArgumentNullException(nameof(richTextParagraph), "IRichTextParagraph cannot be null");
            _i = richTextParagraph as Renga.IRichTextParagraph;
            if (_i == null)
                throw new ArgumentException("Object does not implement IRichTextParagraph interface", nameof(richTextParagraph));
        }

        /// <summary>
        /// Constructor - Creates IRichTextParagraph from typed COM object
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public IRichTextParagraph(Renga.IRichTextParagraph richTextParagraph)
        {
            _i = richTextParagraph ?? throw new ArgumentNullException(nameof(richTextParagraph));
        }

        ~IRichTextParagraph()
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
        /// Gets the number of tokens in the paragraph
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public int TokenCount
        {
            get
            {
                if (_i == null)
                    throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
                try
                {
                    return _i.TokenCount;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get token count: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Returns the token at the given index as RichTextToken (Text, FontFamily, FontCapSize, FontColor, FontStyle).
        /// See https://help.rengabim.com/api/struct_rich_text_token.html
        /// </summary>
        /// <param name="index">Token index (zero-based)</param>
        /// <returns>RichTextToken wrapper with Text, FontFamily, FontCapSize, FontColor, FontStyle</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public RichTextToken GetToken(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
            try
            {
                var token = _i.GetToken(index);
                return RichTextToken.FromRenga(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get token at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Removes the token at the given index. If the paragraph contains only one token, it cannot be deleted.
        /// </summary>
        /// <param name="index">Token index (zero-based)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void RemoveToken(int index)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
            try
            {
                _i.RemoveToken(index);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to remove token at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Appends the token to the end of the paragraph.
        /// See https://help.rengabim.com/api/interface_i_rich_text_paragraph.html
        /// </summary>
        /// <param name="token">RichTextToken to append (Text, FontFamily, FontCapSize, FontColor, FontStyle)</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void AppendToken(RichTextToken token)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Token cannot be null.");
            try
            {
                _i.AppendToken(token.ToRenga());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to append token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inserts the token at the given index.
        /// See https://help.rengabim.com/api/interface_i_rich_text_paragraph.html
        /// </summary>
        /// <param name="index">Index at which to insert (zero-based)</param>
        /// <param name="token">RichTextToken to insert</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void InsertToken(int index, RichTextToken token)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Token cannot be null.");
            try
            {
                _i.InsertToken(index, token.ToRenga());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to insert token at index {index}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Prepends the token at the beginning of the paragraph.
        /// See https://help.rengabim.com/api/interface_i_rich_text_paragraph.html
        /// </summary>
        /// <param name="token">RichTextToken to prepend</param>
        [dr.IsVisibleInDynamoLibrary(true)]
        public void PrependToken(RichTextToken token)
        {
            if (_i == null)
                throw new InvalidOperationException("IRichTextParagraph interface is not initialized.");
            if (token == null)
                throw new ArgumentNullException(nameof(token), "Token cannot be null.");
            try
            {
                _i.PrependToken(token.ToRenga());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to prepend token: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Checks if the interface is properly initialized
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public bool IsInitialized() => _i != null && !_disposed;
    }
}
