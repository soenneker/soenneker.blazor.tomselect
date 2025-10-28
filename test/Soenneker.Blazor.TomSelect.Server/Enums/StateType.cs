namespace Soenneker.Blazor.TomSelect.Server.Enums;

public class StateType
{
    public string Name { get; }
    public string Value { get; }

    private StateType(string name)
    {
        Name = name;
        Value = name;
    }

    public static readonly StateType Alabama = new(nameof(Alabama));
    public static readonly StateType Alaska = new(nameof(Alaska));
    public static readonly StateType Arizona = new(nameof(Arizona));
    public static readonly StateType Arkansas = new(nameof(Arkansas));
    public static readonly StateType California = new(nameof(California));
    public static readonly StateType Colorado = new(nameof(Colorado));
    public static readonly StateType Connecticut = new(nameof(Connecticut));
    public static readonly StateType Delaware = new(nameof(Delaware));
    public static readonly StateType DistrictOfColumbia = new(nameof(DistrictOfColumbia));
    public static readonly StateType Florida = new(nameof(Florida));
    public static readonly StateType Georgia = new(nameof(Georgia));
    public static readonly StateType Hawaii = new(nameof(Hawaii));
    public static readonly StateType Idaho = new(nameof(Idaho));
    public static readonly StateType Illinois = new(nameof(Illinois));
    public static readonly StateType Indiana = new(nameof(Indiana));
    public static readonly StateType Iowa = new(nameof(Iowa));
    public static readonly StateType Kansas = new(nameof(Kansas));
    public static readonly StateType Kentucky = new(nameof(Kentucky));
    public static readonly StateType Louisiana = new(nameof(Louisiana));
    public static readonly StateType Maine = new(nameof(Maine));
    public static readonly StateType Maryland = new(nameof(Maryland));
    public static readonly StateType Massachusetts = new(nameof(Massachusetts));
    public static readonly StateType Michigan = new(nameof(Michigan));
    public static readonly StateType Minnesota = new(nameof(Minnesota));
    public static readonly StateType Mississippi = new(nameof(Mississippi));
    public static readonly StateType Missouri = new(nameof(Missouri));
    public static readonly StateType Montana = new(nameof(Montana));
    public static readonly StateType Nebraska = new(nameof(Nebraska));
    public static readonly StateType Nevada = new(nameof(Nevada));
    public static readonly StateType NewHampshire = new(nameof(NewHampshire));
    public static readonly StateType NewJersey = new(nameof(NewJersey));
    public static readonly StateType NewMexico = new(nameof(NewMexico));
    public static readonly StateType NewYork = new(nameof(NewYork));
    public static readonly StateType NorthCarolina = new(nameof(NorthCarolina));
    public static readonly StateType NorthDakota = new(nameof(NorthDakota));
    public static readonly StateType Ohio = new(nameof(Ohio));
    public static readonly StateType Oklahoma = new(nameof(Oklahoma));
    public static readonly StateType Oregon = new(nameof(Oregon));
    public static readonly StateType Pennsylvania = new(nameof(Pennsylvania));
    public static readonly StateType PuertoRico = new(nameof(PuertoRico));
    public static readonly StateType RhodeIsland = new(nameof(RhodeIsland));
    public static readonly StateType SouthCarolina = new(nameof(SouthCarolina));
    public static readonly StateType SouthDakota = new(nameof(SouthDakota));
    public static readonly StateType Tennessee = new(nameof(Tennessee));
    public static readonly StateType Texas = new(nameof(Texas));
    public static readonly StateType Utah = new(nameof(Utah));
    public static readonly StateType Vermont = new(nameof(Vermont));
    public static readonly StateType Virginia = new(nameof(Virginia));
    public static readonly StateType Washington = new(nameof(Washington));
    public static readonly StateType WestVirginia = new(nameof(WestVirginia));
    public static readonly StateType Wisconsin = new(nameof(Wisconsin));
    public static readonly StateType Wyoming = new(nameof(Wyoming));

    public static IEnumerable<StateType> List()
    {
        return new[]
        {
            Alabama, Alaska, Arizona, Arkansas, California, Colorado, Connecticut, Delaware,
            DistrictOfColumbia, Florida, Georgia, Hawaii, Idaho, Illinois, Indiana, Iowa,
            Kansas, Kentucky, Louisiana, Maine, Maryland, Massachusetts, Michigan, Minnesota,
            Mississippi, Missouri, Montana, Nebraska, Nevada, NewHampshire, NewJersey,
            NewMexico, NewYork, NorthCarolina, NorthDakota, Ohio, Oklahoma, Oregon,
            Pennsylvania, PuertoRico, RhodeIsland, SouthCarolina, SouthDakota, Tennessee,
            Texas, Utah, Vermont, Virginia, Washington, WestVirginia, Wisconsin, Wyoming
        };
    }
}
