namespace Grains.Grains;

public interface IHello : IGrainWithIntegerKey
{
    ValueTask<string> SayHello(string greeting);
}