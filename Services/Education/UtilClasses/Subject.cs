
namespace MPTC_API.Services.Education.UtilClasses;
public class Subject
{
    public string Name { get; set; }
    public List<Section> Sections { get; set; }

    public Subject(){
        this.Name = "";
        this.Sections = new List<Section>();
    }
}
