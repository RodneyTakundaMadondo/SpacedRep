using DocumentFormat.OpenXml.Packaging;
using Google.GenAI.Types;
using System.IO.Compression;
using System.Text;


namespace StudyReminder.Helpers
{
    public static class DocumentHelper
    {
        public static async Task<string> GetWordText(string cloudinaryUrl)
        {
            try
            {
                using var httpClient = new HttpClient();
                var stream = await httpClient.GetStreamAsync(cloudinaryUrl);

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, false))
                {
                    StringBuilder sb = new StringBuilder();
                    var body = wordDoc.MainDocumentPart.Document.Body;
                    foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                    {
                        sb.Append(text.Text);
                    }
                    return sb.ToString();
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
         
        }
        public async static Task<string> GetTextFileText(string cloudinaryUrl)
        {

            try
            {
                using var httpClient = new HttpClient();
                string text = await httpClient.GetStringAsync(cloudinaryUrl);
                return text;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public static async Task<byte[]> GetPdfBytes(string cloudinaryUrl)
        {
            using var httpClient = new HttpClient();
            byte[] pdfBytes = await httpClient.GetByteArrayAsync(cloudinaryUrl);
            return pdfBytes;

        }
        public static string GetFileType(string fileName)
        {
            string FileType = "";
            int startIndex = fileName.IndexOf(".");
            string type = fileName.Substring(startIndex + 1);
            switch (type?.ToLowerInvariant())  // ← normalize to lowercase to handle "PDF", "Pdf", etc.
            {
                // Documents - extremely common
                case "pdf":
                    FileType = "PDF Document";
                    break;

                case "docx":
                    FileType = "Microsoft Word Document";
                    break;

                case "doc":
                    FileType = "Microsoft Word Document (older format)";
                    break;

                // Spreadsheets
                case "xlsx":
                    FileType = "Microsoft Excel Spreadsheet";
                    break;

                case "xls":
                    FileType = "Microsoft Excel Spreadsheet (older format)";
                    break;

                // Presentations
                case "pptx":
                    FileType = "Microsoft PowerPoint Presentation";
                    break;

                case "ppt":
                    FileType = "Microsoft PowerPoint Presentation (older format)";
                    break;

                // Images - very frequent in uploads
                case "jpg":
                case "jpeg":
                    FileType = "JPEG Image";
                    break;

                case "png":
                    FileType = "PNG Image";
                    break;

                case "gif":
                    FileType = "GIF Image";
                    break;

                // Archives & compressed
                case "zip":
                    FileType = "ZIP Archive";
                    break;

                // Text & data
                case "txt":
                    FileType = "Plain Text File";
                    break;

                case "csv":
                    FileType = "Comma-Separated Values (CSV)";
                    break;

                case "rtf":
                    FileType = "Rich Text Format";
                    break;

                // Fallback for unknown types
                default:
                    FileType = "Unknown File Type";
                    // Optionally: FileType = $"{type?.ToUpperInvariant()} File"; 
                    break;
            }
            return FileType;
        }

        public static string GetExtension(string fileName)
        {
            string FileType = "";
            int startIndex = fileName.IndexOf(".");
            string type = fileName.Substring(startIndex + 1);
            return type;
        }

    
        
        public static bool IsPdf(IFormFile file)
        {
            try
            {

                using var stream = file.OpenReadStream();
                using var reader = new BinaryReader(stream);

                var header = reader.ReadBytes(4);
                var headerText = System.Text.Encoding.ASCII.GetString(header);


                return headerText == "%PDF";
            }
            catch
            {
                return false;
            }
        }
        public static bool IsWordDoc(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var zip = new ZipArchive(stream, ZipArchiveMode.Read);

                var hasContentTypes = zip.Entries.Any(e => e.FullName == "[Content_Types].xml");
                var hasDocument = zip.Entries.Any(e => e.FullName == "word/document.xml");

                if (!hasContentTypes || !hasDocument)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidTextDoc(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, Encoding.UTF8, false);
                var content = reader.ReadToEnd();
                bool hasBinary = content.Any(c => char.IsControl(c) && c != '\n' && c != '\r' && c != '\t');
                return hasBinary ? false : true;
            }
            catch
            {
                return false;
            }
        }

       
    }
}
