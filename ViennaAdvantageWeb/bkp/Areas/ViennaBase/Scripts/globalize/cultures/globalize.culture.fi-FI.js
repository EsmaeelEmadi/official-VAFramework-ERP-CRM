/*
 * Globalize Culture fi-FI
 *
 * http://github.com/jquery/globalize
 *
 * Copyright Software Freedom Conservancy, Inc.
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * This file was generated by the Globalize Culture Generator
 * Translation: bugs found in this file need to be fixed in the generator
 */

(function( window, undefined ) {

var Globalize;

if ( typeof require !== "undefined" &&
	typeof exports !== "undefined" &&
	typeof module !== "undefined" ) {
	// Assume CommonJS
	Globalize = require( "globalize" );
} else {
	// Global variable
	Globalize = window.Globalize;
}

Globalize.addCultureInfo( "fi-FI", "default", {
	name: "fi-FI",
	englishName: "Finnish (Finland)",
	nativeName: "suomi (Suomi)",
	language: "fi",
	numberFormat: {
		",": " ",
		".": ",",
		negativeInfinity: "-INF",
		positiveInfinity: "INF",
		percent: {
			",": " ",
			".": ","
		},
		currency: {
			pattern: ["-n $","n $"],
			",": " ",
			".": ",",
			symbol: "€"
		}
	},
	calendars: {
		standard: {
			"/": ".",
			firstDay: 1,
			days: {
				names: ["sunnuntai","maanantai","tiistai","keskiviikko","torstai","perjantai","lauantai"],
				namesAbbr: ["su","ma","ti","ke","to","pe","la"],
				namesShort: ["su","ma","ti","ke","to","pe","la"]
			},
			months: {
				names: ["tammikuu","helmikuu","maaliskuu","huhtikuu","toukokuu","kesäkuu","heinäkuu","elokuu","syyskuu","lokakuu","marraskuu","joulukuu",""],
				namesAbbr: ["tammi","helmi","maalis","huhti","touko","kesä","heinä","elo","syys","loka","marras","joulu",""]
			},
			AM: null,
			PM: null,
			patterns: {
				d: "d.M.yyyy",
				D: "d. MMMM'ta 'yyyy",
				t: "H:mm",
				T: "H:mm:ss",
				f: "d. MMMM'ta 'yyyy H:mm",
				F: "d. MMMM'ta 'yyyy H:mm:ss",
				M: "d. MMMM'ta'",
				Y: "MMMM yyyy"
			}
		}
	},
	messages: {
	    "Connection": "Yhteys",
    "Defaults": "Oletusarvot",
    "Login": "Login",
    "File": "Tiedosto",
    "Exit": "Poistu",
    "Help": "Ohje",
    "About": "About",
    "Host": "Host",
    "Database": "Tietokanta",
    "User": "Käyttäjätunnus",
    "EnterUser": "Anna sovelluksen käyttäjätunnus",
    "Password": "Salasana",
    "EnterPassword": "Anna sovelluksen salasana",
    "Language": "Kieli",
    "SelectLanguage": "Valitse kieli",
    "Role": "Rooli",
    "Client": "Client",
    "Organization": "Organisaatio",
    "Date": "Päivämäärä",
    "Warehouse": "Tietovarasto",
    "Printer": "Tulostin",
    "Connected": "Yhdistetty",
    "NotConnected": "Ei yhteyttä",
    "DatabaseNotFound": "Tietokantaa ei löydy",
    "UserPwdError": "Käyttäjätunnus ja salasana eivät vastaa toisiaan",
    "RoleNotFound": "Roolia ei löydy tai se ei ole täydellinen",
    "Authorized": "Valtuutettu",
    "Ok": "Hyväksy",
    "Cancel": "Peruuta",
    "VersionConflict": "Versioristiriita:",
    "VersionInfo": "Server <> Client",
    "PleaseUpgrade": "Ole hyvä ja aja päivitysohjelma",

    //New Resource

    "Back": "takaisin",
    "SelectRole": "Valitse rooli",
    "SelectOrg": "Valitse Organisaatio",
    "SelectClient": "Valitse asiakas",
    "SelectWarehouse": "Valitse varasto",
    "VerifyUserLanguage": "Tarkista Käyttäjä Kieli",
    "LoadingPreference": "Ladataan etusija",
    "Completed": "valmistunut",
        "RememberMe": "Muista minut",
        "FillMandatoryFields": "Täytä pakolliset kentät",
        "BothPwdNotMatch": "Molempien salasanojen on vastattava toisiaan.",
        "mustMatchCriteria": "Salasanan vähimmäispituus on 5. Salasanassa on oltava vähintään yksi iso kirjain, 1 pieni kirjain, yksi erikoismerkki (@ $!% *? &) Ja yksi numero. Salasanan on alkaa merkillä.",
        "NotLoginUser": "Käyttäjä ei voi kirjautua järjestelmään",
        "MaxFailedLoginAttempts": "Käyttäjätili on lukittu. Suurimmat epäonnistuneet kirjautumisyritykset ylittävät määritellyn rajan. Ota yhteyttä järjestelmänvalvojaan.",
        "UserNotFound": "Käyttäjätunnus on väärä.",
        "RoleNotDefined": "Tälle käyttäjälle ei määritetty roolia",
        "oldNewSamePwd": "vanhan ja uuden salasanan on oltava erilaiset.",
        "NewPassword": "uusi salasana",
        "NewCPassword": "Vahvista uusi salasana",
        "EnterOTP": "Syötä OTP",
        "WrongOTP": "Väärä OTP kirjoitettu",
        "ScanQRCode": "Skannaa koodi Google Authenticatorilla",
        "EnterVerCode": "Kirjoita mobiilisovelluksesi luoma OTP",
	}
});

}( this ));
