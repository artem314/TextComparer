using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TextComparer
{
    class TxtFileService: IFileService
    {
        public async Task<Text> OpenAsync(string filename)
        {
            Text text = new Text();
            using (StreamReader sr = new StreamReader(filename))
            {
                string line = await sr.ReadToEndAsync();
                text.Content = line;
                text.Title = Path.GetFileNameWithoutExtension(filename);
            }

            return text;
        }
    }
}
