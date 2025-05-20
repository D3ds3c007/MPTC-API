
namespace MPTC_API.Services.Education.UtilClasses;
public class StudentAnswer
{
    public string Name { get; set; }
    public List<Section> Sections { get; set; }

    public StudentAnswer(){
        this.Name = "";
        this.Sections = new List<Section>();
    }
}
