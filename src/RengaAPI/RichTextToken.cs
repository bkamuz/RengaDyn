using System;
using System.Collections.Generic;
using dr = Autodesk.DesignScript.Runtime;
using Renga;

namespace DynRenga.RengaAPI
{
    /// <summary>
    /// Dynamo wrapper for Renga RichTextToken struct.
    /// Represents a text token (structural element of IRichTextParagraph).
    /// See https://help.rengabim.com/api/struct_rich_text_token.html
    /// </summary>
    public class RichTextToken
    {
        /// <summary>
        /// Creates an empty RichTextToken. Set Text, FontFamily, FontCapSize (and optionally FontColor, FontStyle) then use with AppendToken/InsertToken/PrependToken.
        /// </summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public RichTextToken()
        {
            Text = "";
            FontFamily = "Arial";
            FontCapSize = 12f;
            FontColor = new Dictionary<string, int> { { "Red", 0 }, { "Green", 0 }, { "Blue", 0 }, { "Alpha", 255 } };
            FontStyle = new Dictionary<string, bool> { { "Bold", false }, { "Italic", false }, { "Underline", false } };
        }

        /// <summary>
        /// Creates a RichTextToken by text and font. Use the result with IRichTextParagraph.AppendToken, InsertToken or PrependToken.
        /// </summary>
        /// <param name="text">Token text</param>
        /// <param name="fontFamily">Font family name (e.g. Arial)</param>
        /// <param name="fontCapSize">Font cap size (e.g. 12)</param>
        /// <returns>RichTextToken for use with AppendToken/InsertToken/PrependToken</returns>
        [dr.IsVisibleInDynamoLibrary(true)]
        public static RichTextToken ByTextAndFont(string text, string fontFamily, double fontCapSize)
        {
            return new RichTextToken
            {
                Text = text ?? "",
                FontFamily = fontFamily ?? "Arial",
                FontCapSize = (float)fontCapSize,
                FontColor = new Dictionary<string, int> { { "Red", 0 }, { "Green", 0 }, { "Blue", 0 }, { "Alpha", 255 } },
                FontStyle = new Dictionary<string, bool> { { "Bold", false }, { "Italic", false }, { "Underline", false } }
            };
        }

        /// <summary>Token text (BSTR)</summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string Text { get; set; }

        /// <summary>Font family name</summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public string FontFamily { get; set; }

        /// <summary>Font cap size (float)</summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public float FontCapSize { get; set; }

        /// <summary>Font color as dictionary: Red, Green, Blue, Alpha (0–255)</summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, int> FontColor { get; set; }

        /// <summary>Font style as dictionary: Bold, Italic, Underline (bool)</summary>
        [dr.IsVisibleInDynamoLibrary(true)]
        public Dictionary<string, bool> FontStyle { get; set; }

        /// <summary>
        /// Creates RichTextToken from Renga RichTextToken struct (COM interop).
        /// Renga.RichTextToken is a struct (value type) and must not be compared with null.
        /// </summary>
        internal static RichTextToken FromRenga(dynamic rengaToken)
        {
            var t = new RichTextToken();
            try { t.Text = (rengaToken.Text ?? "").ToString(); } catch { t.Text = ""; }
            try { t.FontFamily = (rengaToken.FontFamily ?? "").ToString(); } catch { t.FontFamily = ""; }
            try { t.FontCapSize = (float)rengaToken.FontCapSize; } catch { t.FontCapSize = 0f; }
            try
            {
                var c = rengaToken.FontColor;
                t.FontColor = new Dictionary<string, int>
                {
                    { "Red", (int)c.Red }, { "Green", (int)c.Green }, { "Blue", (int)c.Blue }, { "Alpha", (int)c.Alpha }
                };
            }
            catch { t.FontColor = new Dictionary<string, int> { { "Red", 0 }, { "Green", 0 }, { "Blue", 0 }, { "Alpha", 255 } }; }
            try
            {
                var s = rengaToken.FontStyle;
                t.FontStyle = new Dictionary<string, bool>
                {
                    { "Bold", (bool)s.Bold }, { "Italic", (bool)s.Italic }, { "Underline", (bool)s.Underline }
                };
            }
            catch { t.FontStyle = new Dictionary<string, bool> { { "Bold", false }, { "Italic", false }, { "Underline", false } }; }
            return t;
        }

        /// <summary>
        /// Converts this wrapper to Renga.RichTextToken struct for AppendToken/InsertToken/PrependToken.
        /// </summary>
        internal Renga.RichTextToken ToRenga()
        {
            var r = new Renga.RichTextToken();
            r.Text = Text ?? "";
            r.FontFamily = FontFamily ?? "";
            r.FontCapSize = FontCapSize;
            if (FontColor != null)
            {
                r.FontColor = new Renga.Color
                {
                    Red = (ushort)(FontColor.ContainsKey("Red") ? FontColor["Red"] : 0),
                    Green = (ushort)(FontColor.ContainsKey("Green") ? FontColor["Green"] : 0),
                    Blue = (ushort)(FontColor.ContainsKey("Blue") ? FontColor["Blue"] : 0),
                    Alpha = (ushort)(FontColor.ContainsKey("Alpha") ? FontColor["Alpha"] : 255)
                };
            }
            if (FontStyle != null)
            {
                r.FontStyle = new Renga.FontStyle
                {
                    Bold = (sbyte)((FontStyle.ContainsKey("Bold") && FontStyle["Bold"]) ? 1 : 0),
                    Italic = (sbyte)((FontStyle.ContainsKey("Italic") && FontStyle["Italic"]) ? 1 : 0),
                    Underline = (sbyte)((FontStyle.ContainsKey("Underline") && FontStyle["Underline"]) ? 1 : 0)
                };
            }
            return r;
        }
    }
}
