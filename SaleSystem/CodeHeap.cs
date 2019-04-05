namespace Interface_Reception_Ribbon.Sale
{
    public class CodeHeap
    {
        string[] _codes = new string[8];
        int _index = 0;
        int _len = 0;
        public int Len
        {
            get
            {
                return _len;
            }
        }

        public void Push(string code)
        {
            _codes[_len] = code;
            _len++;
            if (_len > 7) { _len = 0; }
        }

        public void Pop(out string code)
        {
            code = _codes[_index];
            _index++;
            _len--;
            if (_index > 7) { _index = 0; }
            if (_len < 0) { _len = 0; }
        }

    }
}
