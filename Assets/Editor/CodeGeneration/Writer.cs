using System.Text;

namespace Chocolate4.CodeGeneration
{
    internal struct Writer
    {
        private const int SpacesPerIndentLevel = 4;

        public StringBuilder buffer;
        public int indentLevel;

        public Writer(StringBuilder buffer)
        {
            this.buffer = buffer;
            indentLevel = 0;
        }

        public void BeginBlock(string extra = "")
        {
            buffer.Append("{" + extra + "\n");
            indentLevel++;
        }

        public void EndBlock(string extra = "")
        {
            indentLevel--;
            buffer.Append("}" + extra +"\n");
        }

        public void WriteLine()
        {
            buffer.Append('\n');
        }

        public void WriteLine(string text)
        {
            WriteIndent();
            buffer.Append(text);
            WriteLine();
        }

        public void Write(string text)
        {
            WriteIndent();
            buffer.Append(text);
        }

        public void WriteIndent()
        {
            for (var i = 0; i < indentLevel; i++)
            {
                for (var n = 0; n < SpacesPerIndentLevel; n++)
                    buffer.Append(' ');
            }
        }
    }
}
