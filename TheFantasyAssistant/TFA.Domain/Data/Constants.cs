namespace TFA.Domain.Data;

public sealed class TeamNames
{
    public const string ARSENAL = "Arsenal";
    public const string MAN_CITY = "Man City";
    public const string MAN_UTD = "Man Utd";
    public const string LIVERPOOL = "Liverpool";
    public const string NEWCASTLE = "Newcastle";
    public const string FULHAM = "Fulham";
    public const string BRIGHTON = "Brighton";
    public const string BRENTFORD = "Brentford";
    public const string CHELSEA = "Chelsea";
    public const string ASTON_VILLA = "Aston Villa";
    public const string CRYSTAL_PALACE = "Crystal Palace";
    public const string WOLVERHAMPTHON = "Wolves";
    public const string NOTTINGHAM_FOREST = "Nott'm Forest";
    public const string LEICESTER = "Leicester";
    public const string WEST_HAM = "West Ham";
    public const string LEEDS = "Leeds";
    public const string EVERTON = "Everton";
    public const string SOUTHAMPTHON = "Southampton";
    public const string BOURNEMOUTH = "Bournemouth";
    public const string TOTTENHAM = "Spurs";
    public const string LUTON = "Luton";
    public const string SHEFFIELD_UTD = "Sheffield Utd";

    public const string AIK = "AIK";
    public const string DJURGARDEN = "Djurgården";
    public const string HAMMARBY = "Hammarby";
    public const string SIRIUS = "IK Sirius";
    public const string VARBERG = "Varbergs BoIS";
    public const string MJALLBY = "Mjällby AIF";
    public const string HALMSTAD = "Halmstads BK";
    public const string DEGERFORS = "Degerfors IF";
    public const string VARNAMO = "IFK Värnamo";
    public const string BROMMAPOJKARNA = "IF Brommapojkarna";
    public const string MALMO = "Malmö FF";
    public const string GOTEBORG = "IFK Göteborg";
    public const string ELFSBORG = "IF Elfsborg";
    public const string HACKEN = "BK Häcken";
    public const string NORRKOPING = "IFK Norrköping";
    public const string KALMAR = "Kalmar FF";
}

public sealed class PlayerPositionDisplayNames
{
    public const string Goalkeeper = "GKP";
    public const string Defender = "DEF";
    public const string Midfielder = "MID";
    public const string Attacker = "FWD";
}

public sealed class PlayerStatuses
{
    /// <summary>
    /// Player is available
    /// </summary>
    public const string Available = "a";

    /// <summary>
    /// Player is no longer part of the game.
    /// This is set for players that are sold or loaned out with no chance of coming back during the season.
    /// </summary>
    public const string Unavailable = "u";

    /// <summary>
    /// Player is not available at the moment but might return in a near future.
    /// This is mostly set for players that are loaned out short term.
    /// </summary>
    public const string NotAvailable = "n";

    /// <summary>
    /// Player is injured.
    /// </summary>
    public const string Injured = "i";

    /// <summary>
    /// Player is doubtful.
    /// </summary>
    public const string Doubtful = "d";

    /// <summary>
    /// Player is suspended
    /// </summary>
    public const string Suspended = "s";
}

public sealed class ChipNames
{
    public const string Wildcard = "wildcard";
    public const string BenchBoost = "bboost";
    public const string FreeHit = "freehit";
    public const string TripleCaptain = "3xc";
    public const string ParkTheBus = "pdbus";
    public const string TwoCaptains = "2capt";
    public const string LoanTeam = "uteam";
}

public sealed class FantasyLastGameweek
{
    public const int FPL = 38;
    public const int Allsvenskan = 30;
}

public sealed class ExpectedLeagueTeamCount
{
    public const int PremierLeague = 20;
    public const int Allsvenskan = 16;
}