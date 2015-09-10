using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Vml.Office;

using d = DocumentFormat.OpenXml.Drawing;
using V = DocumentFormat.OpenXml.Vml;
using Ovml = DocumentFormat.OpenXml.Vml.Office;

namespace PYS.Lib
{
    public class WordGenerator
    {
        private const double EMU_PER_PIXEL = 9525;
        private const string GRAPHIC_DATA_URI = @"http://schemas.openxmlformats.org/drawingml/2006/picture";
        static string imageRelationshipID = null;

        public static void Test()
        {
            int[] a = new int[2];
            List<TextPositon> listTextPositon = new List<TextPositon>();
            listTextPositon.Add(new TextPositon() { X = 0, Y = 130, Text = "http://www.weddingchannel.com" });
            listTextPositon.Add(new TextPositon() { X = 0, Y = 285, Text = "http://test.registry.theknot.comtheknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 0, Y = 435, Text = "http://test.abc.com" });
            listTextPositon.Add(new TextPositon() { X = 0, Y = 590, Text = "http://test.registry.theknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 0, Y = 740, Text = "http://test.registry.theknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 270, Y = 130, Text = "http://www.weddingchannel.com" });
            listTextPositon.Add(new TextPositon() { X = 270, Y = 285, Text = "http://test.registry.theknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 270, Y = 435, Text = "http://test.registry.theknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 270, Y = 590, Text = "http://test.registry.theknot.com/couplesearch" });
            listTextPositon.Add(new TextPositon() { X = 270, Y = 740, Text = "http://test.registry.theknot.com/couplesearch" });
            string filePath = string.Format("{0}.docx", DateTime.Now.ToString("yyyyMMddhhmmss"));
            BuildDocument(filePath, listTextPositon);
        }

        private static void BuildDocument(string fileName, List<TextPositon> listTextPositon)
        {
            using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Create(fileName, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainDocumentPart = wordprocessingDocument.AddMainDocumentPart();
                Document doc = new Document();
                Body body = new Body();
                HeaderPart headerPart = mainDocumentPart.AddNewPart<HeaderPart>();
                string headerRelationshipID = mainDocumentPart.GetIdOfPart(headerPart);
                Paragraph imageParagraph = AddImageParagraph(mainDocumentPart, listTextPositon);

                Text text = new Text("THIS IS A WONDERFUL WORLD!");
                /*      */
                DocumentBackground documentBackground = new DocumentBackground() { Color = "FF0000" };
                DocumentFormat.OpenXml.Vml.Background background = new DocumentFormat.OpenXml.Vml.Background()
                {
                    Id = "_x0000_s1025",
                    BlackWhiteMode = BlackAndWhiteModeValues.HighContrast,
                    TargetScreenSize = ScreenSizeValues.Sz1024x768
                };
                DocumentFormat.OpenXml.Vml.Fill fill = new DocumentFormat.OpenXml.Vml.Fill()
                {
                    Title = "TTTTTT",
                    Recolor = true,
                    RelationshipId = imageRelationshipID,
                    Color = "FF0000"
                };
                background.Fill = fill;

                documentBackground.Background = background;
                documentBackground.Append(imageParagraph);

                background.Append(fill);
                documentBackground.Append(background);

                doc.DocumentBackground = documentBackground;


                mainDocumentPart.Document = GenerateDocument();
                mainDocumentPart.Document.Save();
                wordprocessingDocument.Close();
            }
        }

        private static Paragraph AddImageParagraph(MainDocumentPart mainDocumentPart, List<TextPositon> listTextPositon)
        {
            ImagePart imagePart = mainDocumentPart.AddImagePart(ImagePartType.Jpeg);
            imageRelationshipID = mainDocumentPart.GetIdOfPart(imagePart);
            using (Stream imgStream = imagePart.GetStream())
            {
                System.Drawing.Bitmap logo = new System.Drawing.Bitmap(string.Format(@"{0}\Resources\Wedding_EnclosureCards.jpg", Environment.CurrentDirectory));
                foreach (var item in listTextPositon)
                {
                    System.Drawing.Font font = new System.Drawing.Font("Thaoma", 10, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(logo);
                    float actualWidth = g.MeasureString(item.Text, font).Width;
                    System.Drawing.RectangleF rectf = new System.Drawing.RectangleF((330 - actualWidth) / 2 + item.X, item.Y, 330, 50);

                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                    g.DrawString(item.Text, font, System.Drawing.Brushes.Gray, rectf);

                    g.Flush();
                }
                logo.Save(imgStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            Paragraph p = new Paragraph();
            Run r = new Run();
            Drawing drawing = BuildImage(imageRelationshipID, "sw.gif", 600, 850);
            r.Append(drawing);
            p.Append(r);
            return p;
        }

        private static Drawing BuildImage(string imageRelationshipID, string imageName, int pixelWidth, int pixelHeight)
        {
            int emuWidth = (int)(pixelWidth * EMU_PER_PIXEL);
            int emuHeight = (int)(pixelHeight * EMU_PER_PIXEL);
            Drawing drawing = new Drawing();
            d.Wordprocessing.Inline inline = new d.Wordprocessing.Inline { DistanceFromTop = 0, DistanceFromBottom = 0, DistanceFromLeft = 0, DistanceFromRight = 0 };
            d.Wordprocessing.Anchor anchor = new d.Wordprocessing.Anchor();
            d.Wordprocessing.SimplePosition simplePos = new d.Wordprocessing.SimplePosition { X = 0, Y = 0 };
            d.Wordprocessing.Extent extent = new d.Wordprocessing.Extent { Cx = emuWidth, Cy = emuHeight };
            d.Wordprocessing.DocProperties docPr = new d.Wordprocessing.DocProperties { Id = 1, Name = imageName };
            d.Graphic graphic = new d.Graphic();
            // We don’t have to hard code a URI anywhere else in the document but if we don’t do it here 
            // we end up with a corrupt document.
            d.GraphicData graphicData = new d.GraphicData { Uri = GRAPHIC_DATA_URI };
            d.Pictures.Picture pic = new d.Pictures.Picture();
            d.Pictures.NonVisualPictureProperties nvPicPr = new d.Pictures.NonVisualPictureProperties();
            d.Pictures.NonVisualDrawingProperties cNvPr = new d.Pictures.NonVisualDrawingProperties { Id = 2, Name = imageName };
            d.Pictures.NonVisualPictureDrawingProperties cNvPicPr = new d.Pictures.NonVisualPictureDrawingProperties();
            d.Pictures.BlipFill blipFill = new d.Pictures.BlipFill();
            d.Blip blip = new d.Blip { Embed = imageRelationshipID };
            d.Stretch stretch = new d.Stretch();
            d.FillRectangle fillRect = new d.FillRectangle();
            d.Pictures.ShapeProperties spPr = new d.Pictures.ShapeProperties();
            d.Transform2D xfrm = new d.Transform2D();
            d.Offset off = new d.Offset { X = 0, Y = 0 };
            d.Extents ext = new d.Extents { Cx = emuWidth, Cy = emuHeight };
            d.PresetGeometry prstGeom = new d.PresetGeometry { Preset = d.ShapeTypeValues.Rectangle };
            d.AdjustValueList avLst = new d.AdjustValueList();
            xfrm.Append(off);
            xfrm.Append(ext);
            prstGeom.Append(avLst);
            stretch.Append(fillRect);
            spPr.Append(xfrm);
            spPr.Append(prstGeom);
            blipFill.Append(blip);
            blipFill.Append(stretch);
            nvPicPr.Append(cNvPr);
            nvPicPr.Append(cNvPicPr);
            pic.Append(nvPicPr);
            pic.Append(blipFill);
            pic.Append(spPr);
            graphicData.Append(pic);
            graphic.Append(graphicData);
            inline.Append(extent);
            inline.Append(docPr);
            inline.Append(graphic);
            drawing.Append(inline);
            return drawing;
        }

        public static Document GenerateDocument()
        {
            Document document1 = new Document() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "w14 wp14" } };
            document1.AddNamespaceDeclaration("wpc", "http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas");
            document1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            document1.AddNamespaceDeclaration("o", "urn:schemas-microsoft-com:office:office");
            document1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            document1.AddNamespaceDeclaration("m", "http://schemas.openxmlformats.org/officeDocument/2006/math");
            document1.AddNamespaceDeclaration("v", "urn:schemas-microsoft-com:vml");
            document1.AddNamespaceDeclaration("wp14", "http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing");
            document1.AddNamespaceDeclaration("wp", "http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing");
            document1.AddNamespaceDeclaration("w10", "urn:schemas-microsoft-com:office:word");
            document1.AddNamespaceDeclaration("w", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            document1.AddNamespaceDeclaration("w14", "http://schemas.microsoft.com/office/word/2010/wordml");
            document1.AddNamespaceDeclaration("wpg", "http://schemas.microsoft.com/office/word/2010/wordprocessingGroup");
            document1.AddNamespaceDeclaration("wpi", "http://schemas.microsoft.com/office/word/2010/wordprocessingInk");
            document1.AddNamespaceDeclaration("wne", "http://schemas.microsoft.com/office/word/2006/wordml");
            document1.AddNamespaceDeclaration("wps", "http://schemas.microsoft.com/office/word/2010/wordprocessingShape");

            DocumentBackground documentBackground1 = new DocumentBackground() { Color = "FF0000" };

            V.Background background1 = new V.Background() { Id = "_x0000_s1025", BlackWhiteMode = Ovml.BlackAndWhiteModeValues.GrayScale, TargetScreenSize = Ovml.ScreenSizeValues.Sz1024x768 };
            V.Fill fill1 = new V.Fill() { Type = V.FillTypeValues.Frame, Title = "Wedding_EnclosureCards", Recolor = true, Color = "FF0000" };

            background1.Append(fill1);

            documentBackground1.Append(background1);

            Body body1 = new Body();

            Paragraph paragraph1 = new Paragraph() { RsidParagraphAddition = "005F6C39", RsidRunAdditionDefault = "005F6C39" };
            BookmarkStart bookmarkStart1 = new BookmarkStart() { Name = "_GoBack", Id = "0" };
            BookmarkEnd bookmarkEnd1 = new BookmarkEnd() { Id = "0" };

            paragraph1.Append(bookmarkStart1);
            paragraph1.Append(bookmarkEnd1);

            SectionProperties sectionProperties1 = new SectionProperties() { RsidR = "005F6C39" };
            PageSize pageSize1 = new PageSize() { Width = (UInt32Value)12240U, Height = (UInt32Value)15840U };
            PageMargin pageMargin1 = new PageMargin() { Top = 1440, Right = (UInt32Value)1440U, Bottom = 1440, Left = (UInt32Value)1440U, Header = (UInt32Value)720U, Footer = (UInt32Value)720U, Gutter = (UInt32Value)0U };
            Columns columns1 = new Columns() { Space = "720" };
            DocGrid docGrid1 = new DocGrid() { LinePitch = 360 };

            sectionProperties1.Append(pageSize1);
            sectionProperties1.Append(pageMargin1);
            sectionProperties1.Append(columns1);
            sectionProperties1.Append(docGrid1);

            body1.Append(paragraph1);
            body1.Append(sectionProperties1);

            document1.Append(documentBackground1);
            document1.Append(body1);
            return document1;
        }
    }
}
public class TextPositon
{
    public string Text { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}