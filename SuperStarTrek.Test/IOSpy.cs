using Games.Common.IO;
using Games.Common.Numbers;
using System.Text;

namespace SuperStarTrek.Test
{
    public class IOSpy : IReadWrite
    {
        private StringBuilder _output = new();
        private Queue<(float, float)> _read2NumbersInputs = new();

        public string GetOutput()
        {
            return _output.ToString();
        }

        //public (float, float) Read2Numbers(string prompt)
        //{
        //    throw new NotImplementedException();
        //}

        public void EnqueueRead2Numbers((float, float) value)
        {
            _read2NumbersInputs.Enqueue(value);
        }

        public (float, float) Read2Numbers(string prompt)
        {
            return _read2NumbersInputs.Count > 0
                ? _read2NumbersInputs.Dequeue()
                : throw new InvalidOperationException("No input value provided for Read2Numbers.");
        }

        public (string, string) Read2Strings(string prompt)
        {
            throw new NotImplementedException();
        }

        public (float, float, float) Read3Numbers(string prompt)
        {
            throw new NotImplementedException();
        }

        public (float, float, float, float) Read4Numbers(string prompt)
        {
            throw new NotImplementedException();
        }

        public char ReadCharacter()
        {
            throw new NotImplementedException();
        }

        public float ReadNumber(string prompt)
        {
            throw new NotImplementedException();
        }

        public void ReadNumbers(string prompt, float[] values)
        {
            throw new NotImplementedException();
        }

        public string ReadString(string prompt)
        {
            throw new NotImplementedException();
        }

        public void Write(string message)
        {
            _output.AppendLine(message);
        }

        public void Write(Number value)
        {
            throw new NotImplementedException();
        }

        public void Write(object value)
        {
            throw new NotImplementedException();
        }

        public void Write(string format, params object[] values)
        {
            throw new NotImplementedException();
        }

        public void Write(Stream stream, bool keepOpen = false)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(Number value)
        {
            throw new NotImplementedException();
        }
        public void WriteLine(string message = "")
        {
            _output.AppendLine(message);
        }

        public void WriteLine(object value)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string format, params object[] values)
        {
            throw new NotImplementedException();
        }
    }
}
