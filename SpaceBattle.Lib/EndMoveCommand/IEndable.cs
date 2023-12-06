namespace SpaceBattle.Lib
{
    public interface IEndable
    {
        public InjectCommand cmd { get; }
        public IUObject target { get; }
    }
}
