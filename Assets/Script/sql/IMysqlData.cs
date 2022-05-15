namespace AirplaneView
{
    public interface IMysqlData
    {
        public string GetInsertString(bool haveAutoInc=false);
        public string GetTableHead(bool haveAutoInc=false);
    }
}