namespace BibliotecaMultimedia.Application.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message)
    {}
    
    public BusinessRuleException(string message, Exception? ex) : base(message, ex) {}
}