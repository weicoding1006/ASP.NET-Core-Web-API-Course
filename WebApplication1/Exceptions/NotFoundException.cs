namespace WebApplication1.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name,object key) : base($"{name}({key}) 沒有找到")
        {

        }
    }
}
