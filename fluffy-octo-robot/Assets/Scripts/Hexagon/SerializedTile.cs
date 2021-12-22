public class SerializedTile
{

    HexCoordinates coordinates;
    int height;

    public SerializedTile(HexCoordinates coordinates, int height)
    {
        this.coordinates = coordinates;
        this.height = height;
    }

    public HexCoordinates GetCoordinates()
    {
        return coordinates;
    }

    public void SetCoordinates(HexCoordinates coordinates)
    {
        this.coordinates = coordinates;
    }

    public int GetHeight()
    {
        return height;
    }

    public void SetHeight(int height)
    {
        this.height = height;
    }

    override
    public string ToString()
    {
        return "Coordinates: " + coordinates + " height: " + height;
    }

}
