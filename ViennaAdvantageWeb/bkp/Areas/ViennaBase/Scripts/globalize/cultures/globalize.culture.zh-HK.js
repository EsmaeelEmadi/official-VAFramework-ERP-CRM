/*
 * Globalize Culture zh-HK
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

Globalize.addCultureInfo( "zh-HK", "default", {
	name: "zh-HK",
	englishName: "Chinese (Traditional, Hong Kong S.A.R.)",
	nativeName: "中文(香港特別行政區)",
	language: "zh-CHT",
	numberFormat: {
		"NaN": "非數字",
		negativeInfinity: "負無窮大",
		positiveInfinity: "正無窮大",
		percent: {
			pattern: ["-n%","n%"]
		},
		currency: {
			symbol: "HK$"
		}
	},
	calendars: {
		standard: {
			days: {
				names: ["星期日","星期一","星期二","星期三","星期四","星期五","星期六"],
				namesAbbr: ["週日","週一","週二","週三","週四","週五","週六"],
				namesShort: ["日","一","二","三","四","五","六"]
			},
			months: {
				names: ["一月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","十二月",""],
				namesAbbr: ["一月","二月","三月","四月","五月","六月","七月","八月","九月","十月","十一月","十二月",""]
			},
			AM: ["上午","上午","上午"],
			PM: ["下午","下午","下午"],
			eras: [{"name":"公元","start":null,"offset":0}],
			patterns: {
				d: "d/M/yyyy",
				D: "yyyy'年'M'月'd'日'",
				t: "H:mm",
				T: "H:mm:ss",
				f: "yyyy'年'M'月'd'日' H:mm",
				F: "yyyy'年'M'月'd'日' H:mm:ss",
				M: "M'月'd'日'",
				Y: "yyyy'年'M'月'"
			}
		}
	},
	messages: {
	    "Connection": "连接",
	    "Defaults": "默认",
	    "Login": "Vienna 登录",
	    "File": "档案",
	    "Exit": "离开",
	    "Help": "帮助",
	    "About": "关於",
	    "Host": "服务器",
	    "Database": "数据库",
	    "User": "用户ID",
	    "EnterUser": "输入应用用户ID (ID) ",
	    "Password": "密码",
	    "EnterPassword": "输入应用密码",
	    "Language": "语言",
	    "SelectLanguage": "请选择语言",
	    "Role": "角色",
	    "Client": "客户端",
	    "Organization": "组织",
	    "Date": "日期",
	    "Warehouse": "仓库",
	    "Printer": "打印机",
	    "Connected": "已连接",
	    "NotConnected": "未连接",
	    "DatabaseNotFound": "未找到数据库",
	    "UserPwdError": "用户密码不一致",
	    "RoleNotFound": "未找到角色/角色未完成",
	    "Authorized": "已授权",
	    "Ok": "确定",
	    "Cancel": "取消",
	    "VersionConflict": "版本冲突:",
	    "VersionInfo": "服务器 <> 客户端",
	    "PleaseUpgrade": "请从服务器下载最新版本",
	    "GoodMorning": "早安",
	    "GoodAfternoon": "午安",
	    "GoodEvening": "晚安",


	    //New Resource

	    "Back": "返回",
	    "SelectRole": "选择角色",
	    "SelectOrg": "选择组织",
	    "SelectClient": "选择客户端",
	    "SelectWarehouse": "选择仓库",
	    "VerifyUserLanguage": "用户及语言验证中",
	    "LoadingPreference": "偏好加载中",
	    "Completed": "已完成",

	    //new
	    "RememberMe": "自动登录",
	    "FillMandatoryFields": "\u64a4\u6d88",
        "BothPwdNotMatch": "Both passwords must match.",
        "mustMatchCriteria": "Minimum length for password is 5. Password must have at least 1 upper case character, 1 lower case character, one special character(@$!%*?&) and one digit. Password must start with character.",
        "NotLoginUser": "User cannot login into system",
        "MaxFailedLoginAttempts": "User account is locked. Maximum failed login attempts exceeds the defined limit. Please contact to administrator.",
        "UserNotFound": "Username is incorrect.",
        "RoleNotDefined": "No role defined for this user",
        "oldNewSamePwd": "old password and new password must be different.",
        "NewPassword": "New Password",
        "NewCPassword": "Confirm New Password",
        "EnterOTP": "Enter OTP",
        "WrongOTP": "Wrong OTP Entered",
        "ScanQRCode": "Scan the code with Google Authenticator",
        "EnterVerCode": "Enter OTP generated by your mobile application"
	}
});

}( this ));
