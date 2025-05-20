
namespace MPTC_API.Services.Education.UtilClasses;

public class AssetNote
{
    public string Name { get; set; }
    public List<Section> Sections { get; set; }

    public AssetNote(){
        this.Name = "";
        this.Sections = new List<Section>();
    }
}
