/*
 * Globalize Culture sl-SI
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

Globalize.addCultureInfo( "sl-SI", "default", {
	name: "sl-SI",
	englishName: "Slovenian (Slovenia)",
	nativeName: "slovenski (Slovenija)",
	language: "sl",
	numberFormat: {
		",": ".",
		".": ",",
		negativeInfinity: "-neskončnost",
		positiveInfinity: "neskončnost",
		percent: {
			pattern: ["-n%","n%"],
			",": ".",
			".": ","
		},
		currency: {
			pattern: ["-n $","n $"],
			",": ".",
			".": ",",
			symbol: "€"
		}
	},
	calendars: {
		standard: {
			"/": ".",
			firstDay: 1,
			days: {
				names: ["nedelja","ponedeljek","torek","sreda","četrtek","petek","sobota"],
				namesAbbr: ["ned","pon","tor","sre","čet","pet","sob"],
				namesShort: ["ne","po","to","sr","če","pe","so"]
			},
			months: {
				names: ["januar","februar","marec","april","maj","junij","julij","avgust","september","oktober","november","december",""],
				namesAbbr: ["jan","feb","mar","apr","maj","jun","jul","avg","sep","okt","nov","dec",""]
			},
			AM: null,
			PM: null,
			patterns: {
				d: "d.M.yyyy",
				D: "d. MMMM yyyy",
				t: "H:mm",
				T: "H:mm:ss",
				f: "d. MMMM yyyy H:mm",
				F: "d. MMMM yyyy H:mm:ss",
				M: "d. MMMM",
				Y: "MMMM yyyy"
			}
		}
	},
	messages: {
	    "Connection": "Povezava",
    "Defaults": "Privzete vrednosti",
    "Login": "Prijava",
    "File": "Datoteka",
    "Exit": "Izhod",
    "Help": "Pomo\u010d",
    "About": "O programu",
    "Host": "Stre\u017enik",
    "Database": "Baza podatkov",
    "User": "Uporabnik",
    "EnterUser": "Vpi\u0161i uporabnika",
    "Password": "Geslo",
    "EnterPassword": "Vpi\u0161i geslo",
    "Language": "Jezik",
    "SelectLanguage": "Izbira jezika",
    "Role": "Vloga",
    "Client": "Podjetje",
    "Organization": "Organizacija",
    "Date": "Datum",
    "Warehouse": "Skladi\u0161\u010de",
    "Printer": "Tiskalnik",
    "Connected": "Povezano",
    "NotConnected": "Ni povezano",
    "DatabaseNotFound": "Ne najdem baze podatkov",
    "UserPwdError": "Geslo ni pravilno",
    "RoleNotFound": "Ne najdem izbrane vloge",
    "Authorized": "Avtoriziran",
    "Ok": "V redu",
    "Cancel": "Prekli\u010di",
    "VersionConflict": "Konflikt verzij",
    "VersionInfo": "Stre\u017enik <> Odjemalec",
    "PleaseUpgrade": "Prosim nadgradite program",


    //New Resource

    "Back": "Nazaj",
    "SelectRole": "Izberite vloge",
    "SelectOrg": "Izberite organizacije",
    "SelectClient": "Izberite odjemalca",
    "SelectWarehouse": "Izberite Warehouse",
    "VerifyUserLanguage": "Preverjanje uporabnika in jezik",
    "LoadingPreference": "Nalaganje preference",
    "Completed": "Zaključen",
        "RememberMe": "Zapomni si me",
        "FillMandatoryFields": "Izpolnite obvezna polja",
        "BothPwdNotMatch": "Obe gesli se morata ujemati.",
        "mustMatchCriteria": "Najmanjša dolžina gesla je 5. Geslo mora imeti vsaj 1 črko, 1 črko, en poseben znak (@ $!% *? &) In eno številko. Geslo se mora začeti z znakom.",
        "NotLoginUser": "Uporabnik se ne more prijaviti v sistem",
        "MaxFailedLoginAttempts": "Uporabniški račun je zaklenjen. Največji neuspeli poskusi prijave presegajo določeno mejo. Obrnite se na skrbnika.",
        "UserNotFound": "Uporabniško ime ni pravilno.",
        "RoleNotDefined": "Za tega uporabnika ni določena nobena vloga",
        "oldNewSamePwd": "Staro in novo geslo morata biti različna.",
        "NewPassword": "novo geslo",
        "NewCPassword": "Potrdite novo geslo",
        "EnterOTP": "Vnesite OTP",
        "WrongOTP": "Vpisan napačen OTP",
        "ScanQRCode": "Optično preglejte kodo Google Authenticator",
        "EnterVerCode": "Vnesite OTP, ki ga ustvari vaša mobilna aplikacija"
	}
});

}( this ));
