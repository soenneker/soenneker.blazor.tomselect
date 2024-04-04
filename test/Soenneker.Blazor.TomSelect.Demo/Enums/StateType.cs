using Soenneker.SmartEnum.Abbreviated;

namespace Soenneker.Blazor.TomSelect.Demo.Enums
{
    public class StateType : AbbreviatedSmartEnum<StateType>
    {
        public static readonly StateType Alabama = new(nameof(Alabama), 1, "AL");
        public static readonly StateType Alaska = new(nameof(Alaska), 2, "AK");
        public static readonly StateType Arizona = new(nameof(Arizona), 3, "AZ");
        public static readonly StateType Arkansas = new(nameof(Arkansas), 4, "AR");
        public static readonly StateType California = new(nameof(California), 5, "CA");
        public static readonly StateType Colorado = new(nameof(Colorado), 6, "CO");
        public static readonly StateType Connecticut = new(nameof(Connecticut), 7, "CT");
        public static readonly StateType Delaware = new(nameof(Delaware), 8, "DE");
        public static readonly StateType DistrictOfColumbia = new(nameof(DistrictOfColumbia), 8, "DC", "District of Columbia");
        public static readonly StateType Florida = new(nameof(Florida), 9, "FL");
        public static readonly StateType Georgia = new(nameof(Georgia), 10, "GA");
        public static readonly StateType Hawaii = new(nameof(Hawaii), 11, "HI");
        public static readonly StateType Idaho = new(nameof(Idaho), 12, "ID");
        public static readonly StateType Illinois = new(nameof(Illinois), 13, "IL");
        public static readonly StateType Indiana = new(nameof(Indiana), 14, "IN");
        public static readonly StateType Iowa = new(nameof(Iowa), 15, "IA");
        public static readonly StateType Kansas = new(nameof(Kansas), 16, "KS");
        public static readonly StateType Kentucky = new(nameof(Kentucky), 17, "KY");
        public static readonly StateType Louisiana = new(nameof(Louisiana), 18, "LA");
        public static readonly StateType Maine = new(nameof(Maine), 19, "ME");
        public static readonly StateType Maryland = new(nameof(Maryland), 20, "MD");
        public static readonly StateType Massachusetts = new(nameof(Massachusetts), 21, "MA");
        public static readonly StateType Michigan = new(nameof(Michigan), 22, "MI");
        public static readonly StateType Minnesota = new(nameof(Minnesota), 23, "MN");
        public static readonly StateType Missouri = new(nameof(Missouri), 24, "MO");
        public static readonly StateType Mississippi = new(nameof(Mississippi), 25, "MS");
        public static readonly StateType Montana = new(nameof(Montana), 26, "MT");
        public static readonly StateType Nebraska = new(nameof(Nebraska), 27, "NE");
        public static readonly StateType Nevada = new(nameof(Nevada), 28, "NV");
        public static readonly StateType NewHampshire = new(nameof(NewHampshire), 29, "NH", "New Hampshire");
        public static readonly StateType NewJersey = new(nameof(NewJersey), 30, "NJ", "New Jersey");
        public static readonly StateType NewMexico = new(nameof(NewMexico), 31, "NM", "New Mexico");
        public static readonly StateType NewYork = new(nameof(NewYork), 32, "NY", "New York");
        public static readonly StateType NorthCarolina = new(nameof(NorthCarolina), 33, "NC", "North Carolina");
        public static readonly StateType NorthDakota = new(nameof(NorthDakota), 34, "ND", "North Dakota");
        public static readonly StateType Ohio = new(nameof(Ohio), 35, "OH");
        public static readonly StateType Oklahoma = new(nameof(Oklahoma), 36, "OK");
        public static readonly StateType Oregon = new(nameof(Oregon), 37, "OR");
        public static readonly StateType Pennsylvania = new(nameof(Pennsylvania), 38, "PA");
        public static readonly StateType PuertoRico = new(nameof(PuertoRico), 39, "PR", "Puerto Rico");
        public static readonly StateType RhodeIsland = new(nameof(RhodeIsland), 40, "RI", "Rhode Island");
        public static readonly StateType SouthCarolina = new(nameof(SouthCarolina), 41, "SC", "South Carolina");
        public static readonly StateType SouthDakota = new(nameof(SouthDakota), 42, "SD", "South Dakota");
        public static readonly StateType Tennessee = new(nameof(Tennessee), 43, "TN");
        public static readonly StateType Texas = new(nameof(Texas), 44, "TX");
        public static readonly StateType Utah = new(nameof(Utah), 45, "UT");
        public static readonly StateType Vermont = new(nameof(Vermont), 46, "VT");
        public static readonly StateType Virginia = new(nameof(Virginia), 47, "VA");
        public static readonly StateType Washington = new(nameof(Washington), 48, "WA");
        public static readonly StateType WestVirginia = new(nameof(WestVirginia), 49, "WV", "West Virginia");
        public static readonly StateType Wisconsin = new(nameof(Wisconsin), 50, "WI");
        public static readonly StateType Wyoming = new(nameof(Wyoming), 51, "WY");

        public StateType(string name, int value, string abbreviation, string? description = null) : base(name, value, abbreviation, true)
        {
            Description = description ?? name;
        }

        public string Description { get; set; }
    }
}
