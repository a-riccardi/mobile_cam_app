using Unity.Jobs;
using Unity.Collections;

public struct ArrayFlipJob : IJob
{
    public int imageWidth;
    public int imageHeight;

    [DeallocateOnJobCompletion]
    [ReadOnly]
    public NativeArray<byte> source;
    [WriteOnly]
    public NativeArray<byte> destination;

    public void Execute()
    {
        //account for the R-G-B sequential structure, to the image is actually three times larger than pixel width
        imageWidth *= 3;
        for (int j = imageHeight; j > 1; j--)
        {
            destination.Slice((imageHeight - j) * imageWidth, imageWidth).CopyFrom(source.Slice((j - 1) * imageWidth, imageWidth));
        }
    }
}
