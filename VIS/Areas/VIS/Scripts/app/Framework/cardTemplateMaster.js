; (function (VIS, $) {

    function VCTMaster() {
        this.frame;
        var self = this;
        var WindowNo = VIS.Env.getWindowNo();
        var btnTemplateBack = null;
        var count = 1;
        var DivTemplate = null;
        var DivCardFieldSec = null;
        var DivCradStep2 = null;
        var DivStyleSec1 = null;
        var DivViewBlock = null;
        var btnLayoutSetting = null;
        var AD_HeaderLayout_ID = null;
        var btnChangeTemplate = null;
        var divTopNavigator = null;
        var spnLastSaved = null;
        var DivGridSec = null;
        var activeSection = null;
        var startRowIndex = null;
        var startCellIndex = null;
        var lastSelectedID = null;
        var mdown = false;
        var chkAllBorderRadius = null;
        var chkAllPadding = null;
        var chkAllMargin = null;
        var chkAllBorder = null;
        var txtCustomStyle = null;
        var txtSQLQuery = null;
        var txtRowGap = null;
        var txtColGap = null;
        var btnAddField = null;
        var btnAddImgField = null;
        var btnAddImgTxtField = null;
        var btn_BlockCancel = null;
        var btnFinesh = null;
        var rowIdx = null;
        var colIdx = null;
        var txtTemplateName = null;
        var addedColPos = [];
        var sectionCount = 2;
        var btnEditIcon = null;
        isNewSection = false;
        var isSystemTemplate = 'Y';

        var gridObj = {
        };


        var measurment = ['px', '%', 'cm', 'mm', 'in', 'pc', 'pt', 'ch', 'em', 'rem', 'vh', 'vw', 'vmin', 'vmax'];
        var editorProp = {
            width: {
                proprty: 'width',
                value: '',
                measurment: true
            },
            minWidth: {
                proprty: 'min-width',
                value: '',
                measurment: true
            },
            height: {
                proprty: 'height',
                value: '',
                measurment: true
            },
            minHeight: {
                proprty: 'min-height',
                value: '',
                measurment: true
            },
            fontSize: {
                proprty: 'font-size',
                value: '',
                measurment: true
            },
            color: {
                proprty: 'color',
                value: '',
                measurment: false
            },
            border: {
                proprty: 'border',
                value: '',
                measurment: true
            },
            borderType: {
                proprty: 'border-style',
                value: '',
                measurment: false
            },
            borderColor: {
                proprty: 'border-color',
                value: '',
                measurment: false
            },
            borderLeft: {
                proprty: 'border-left-width',
                value: '',
                measurment: true
            },
            borderLeftStyle: {
                proprty: 'border-left-style',
                value: '',
                measurment: false
            },
            borderLeftColor: {
                proprty: 'border-left-color',
                value: '',
                measurment: false
            },
            borderTop: {
                proprty: 'border-top-width',
                value: '',
                measurment: true
            },
            borderTopStyle: {
                proprty: 'border-top-style',
                value: '',
                measurment: false
            },
            borderTopColor: {
                proprty: 'border-top-color',
                value: '',
                measurment: false
            },
            borderRight: {
                proprty: 'border-right-width',
                value: '',
                measurment: true
            },
            borderRightStyle: {
                proprty: 'border-right-style',
                value: '',
                measurment: false
            },
            borderRightColor: {
                proprty: 'border-right-color',
                value: '',
                measurment: false
            },
            borderBottom: {
                proprty: 'border-bottom-width',
                value: '',
                measurment: true
            },
            borderBottomStyle: {
                proprty: 'border-bottom-style',
                value: '',
                measurment: false
            },
            borderBottomColor: {
                proprty: 'border-bottom-color',
                value: '',
                measurment: false
            },

            borderRadius: {
                proprty: 'border-radius',
                value: '',
                measurment: true
            },
            borderTopLeftRadius: {
                proprty: 'border-top-left-radius',
                value: '',
                measurment: true
            },
            borderTopRightRadius: {
                proprty: 'border-top-right-radius',
                value: '',
                measurment: false
            },
            borderBottomRightRadius: {
                proprty: 'border-bottom-right-radius',
                value: '',
                measurment: true
            },
            borderBottomLeftRadius: {
                proprty: 'border-bottom-left-radius',
                value: '',
                measurment: true
            },
            padding: {
                proprty: 'padding',
                value: '',
                measurment: true
            },
            paddingLeft: {
                proprty: 'padding-left',
                value: '',
                measurment: true
            },
            paddingTop: {
                proprty: 'padding-top',
                value: '',
                measurment: true
            },
            paddingRight: {
                proprty: 'padding-right',
                value: '',
                measurment: true
            },
            paddingBottom: {
                proprty: 'padding-bottom',
                value: '',
                measurment: true
            },
            margin: {
                proprty: 'margin',
                value: '',
                measurment: true
            },
            marginLeft: {
                proprty: 'margin-left',
                value: '',
                measurment: true
            },
            marginTop: {
                proprty: 'margin-top',
                value: '',
                measurment: false
            },
            marginRight: {
                proprty: 'margin-right',
                value: '',
                measurment: true
            },
            marginBottom: {
                proprty: 'margin-bottom',
                value: '',
                measurment: true
            }, opacity: {
                proprty: 'opacity',
                value: '',
                measurment: false
            }, backgroundColor: {
                proprty: 'background-color',
                value: '',
                measurment: false
            },
            gradientInput: {
                proprty: 'background',
                value: '',
                measurment: false
            },
            gradient: {
                proprty: 'background',
                value: '',
                measurment: false
            },
            boxShadow: {
                proprty: 'box-shadow',
                value: '',
                measurment: true
            },
            flexDirection: {
                proprty: 'flex-direction',
                value: '',
                measurment: false
            },
            bold: {
                proprty: 'font-weight',
                value: 'bold',
                measurment: false
            },
            italic: {
                proprty: 'font-style',
                value: 'italic',
                measurment: false
            },
            underline: {
                proprty: 'text-decoration',
                value: 'underline',
                measurment: false
            },
            justifyLeft: {
                proprty: 'text-align',
                value: 'left',
                measurment: true
            },
            justifyRight: {
                proprty: 'text-align',
                value: 'right',
                measurment: true
            },
            justifyCenter: {
                proprty: 'text-align',
                value: 'center',
                measurment: true
            },
            upperCase: {
                proprty: 'text-transform',
                value: 'uppercase',
                measurment: true
            },
            capitalize: {
                proprty: 'text-transform',
                value: 'capitalize',
                measurment: true
            },
            lowerCase: {
                proprty: 'text-transform',
                value: 'lowercase',
                measurment: true
            },
            flexJustifyCenter: {
                proprty: 'justify-content',
                value: 'center',
                measurment: true
            },
            flexJustifyStart: {
                proprty: 'justify-content',
                value: 'flex-start',
                measurment: true
            },
            flexJustifyEnd: {
                proprty: 'justify-content',
                value: 'flex-end',
                measurment: true
            },
            flexJustifySpaceBetween: {
                proprty: 'justify-content',
                value: 'space-between',
                measurment: true
            },
            flexJustifySpaceAround: {
                proprty: 'justify-content',
                value: 'space-around',
                measurment: true
            },
            flexAlignCenter: {
                proprty: 'align-items',
                value: 'center',
                measurment: true
            },
            flexAlignEnd: {
                proprty: 'align-items',
                value: 'flex-end',
                measurment: true
            },
            flexAlignStart: {
                proprty: 'align-items',
                value: 'flex-start',
                measurment: true
            }
        }

        var root = $('<div style="height:100%"></div>');
        var isBusyRoot = $("<div class='vis-apanel-busy vis-cardviewmainbusy'></div> ");
        root.append(isBusyRoot);

        function loadUI() {
            root.load(VIS.Application.contextUrl + 'CardTemplateMaster/Index/?windowno=' + WindowNo, function (event) {
                btnTemplateBack = root.find('#BtnTemplateBack_' + WindowNo);
                DivTemplate = root.find('#DivTemplate_' + WindowNo);
                DivCardFieldSec = root.find('#DivCardFieldSec_' + WindowNo);
                DivCradStep2 = root.find('#DivCardStep2_' + WindowNo);
                DivViewBlock = root.find('#DivViewBlock_' + WindowNo);
                DivStyleSec1 = root.find('#DivStyleSec1_' + WindowNo);
                btnLayoutSetting = root.find('#BtnLayoutSetting_' + WindowNo);
                btnChangeTemplate = root.find('#BtnChangeTemplate_' + WindowNo);
                divTopNavigator = DivCradStep2.find('.vis-topNavigator');
                DivGridSec = root.find('#DivGridSec_' + WindowNo);
                activeSection = DivViewBlock.find('.section1');
                chkAllBorderRadius = root.find('#chkAllBorderRadius_' + WindowNo);
                chkAllPadding = root.find('#chkAllPadding_' + WindowNo);
                chkAllMargin = root.find('#chkAllMargin_' + WindowNo);
                chkAllBorder = root.find('#chkAllBorder_' + WindowNo);
                txtTemplateName = root.find('#txtTemplateName_' + WindowNo);
                btnFinesh = root.find('#BtnFinesh_' + WindowNo);
                txtCustomStyle = root.find('#txtCustomStyle_' + WindowNo);
                txtSQLQuery = root.find('#txtSQLQuery_' + WindowNo);
                txtRowGap = DivGridSec.find('.rowGap');
                txtColGap = DivGridSec.find('.colGap');
                btn_BlockCancel = root.find('#Btn_BlockCancel_' + WindowNo);
                btnAddField = root.find('#BtnAddField_' + WindowNo);
                btnAddImgField = root.find('#BtnAddImgField_' + WindowNo);
                btnAddImgTxtField = root.find('#BtnAddImgTxtField_' + WindowNo);
                spnLastSaved = root.find('#spnLastSaved_' + WindowNo);
                btnEditIcon = root.find('#btnEditIcon_' + WindowNo);
                btnTemplateBack.hide();
                events();
                getTemplateDesign();
            });
        }


        function events() {
            $('body').mouseup(function (e) {
                mdown = false;
            });

            btnAddField.click(function () {
                divTopNavigator.hide();
                var itm = $('<div class="fieldGroup">'
                    + '<span class="fieldLbl" title="Label" contenteditable="true">Label:</span>'
                    + '<span class="fieldValue" contenteditable="true">Value</span>'
                    + '</div>');
                var blok = DivViewBlock.find('.vis-active-block');
                if (blok.hasClass('grdDiv')) {
                    blok.append(itm);
                }

            });

            btnAddImgField.click(function () {
                divTopNavigator.hide();
                self.show(false);
            });

            btnAddImgTxtField.click(function () {
                divTopNavigator.hide();
                self.show(true);
            });

            btnEditIcon.click(function () {
                divTopNavigator.hide();
                self.show(true,true);
            });

            btnTemplateBack.click(function (e) {
                count++;
                DivTemplate.hide();
                DivCradStep2.find('.vis-two-sec-two').show();
                DivStyleSec1.show();
            });

            btnLayoutSetting.click(function () {
                addSelectedTemplate();
                count++;
                fillcardLayoutfromTemplate();
                DivTemplate.hide();
                DivCradStep2.find('.vis-two-sec-two').show();
                DivStyleSec1.show();
                if (AD_HeaderLayout_ID == 0 && DivGridSec.find('.rowBox').length == 1) {
                    DivGridSec.find('.addGridCol').click();
                    DivGridSec.find('.addGridRow').click();
                }
            });

            btnChangeTemplate.click(function () {
                btnTemplateBack.show();
                divTopNavigator.hide();
                count--;
                DivTemplate.show();
                DivStyleSec1.hide();
                DivCradStep2.find('.vis-two-sec-two').hide();
                scaleTemplate();
            });

            btnFinesh.click(function (e) {
                saveTemplate(e);
            });

            DivTemplate.find('.vis-cardSingleViewTemplate').click(function () {
                DivTemplate.find('.vis-cardSingleViewTemplate').removeClass('vis-active-template');
                $(this).addClass('vis-active-template');
            });


            chkAllBorderRadius.change(function () {
                if ($(this).is(':checked')) {
                    DivStyleSec1.find('.allBorderRadius').removeClass('displayNone');
                    DivStyleSec1.find('.singleBorderRadius').addClass('displayNone');
                } else {
                    DivStyleSec1.find('.allBorderRadius').addClass('displayNone');
                    DivStyleSec1.find('.singleBorderRadius').removeClass('displayNone');
                }
            });

            chkAllPadding.change(function () {
                if ($(this).is(':checked')) {
                    DivStyleSec1.find('.allPadding').removeClass('displayNone');
                    DivStyleSec1.find('.singlePadding').addClass('displayNone');
                } else {
                    DivStyleSec1.find('.allPadding').addClass('displayNone');
                    DivStyleSec1.find('.singlePadding').removeClass('displayNone');
                }
            });

            chkAllMargin.change(function () {
                if ($(this).is(':checked')) {
                    DivStyleSec1.find('.allMargin').removeClass('displayNone');
                    DivStyleSec1.find('.singleMargin').addClass('displayNone');
                } else {
                    DivStyleSec1.find('.allMargin').addClass('displayNone');
                    DivStyleSec1.find('.singleMargin').removeClass('displayNone');
                }
            });

            chkAllBorder.change(function () {
                if ($(this).is(':checked')) {
                    DivStyleSec1.find('.allBorder').removeClass('displayNone');
                    DivStyleSec1.find('.singleBorder').addClass('displayNone');
                } else {
                    DivStyleSec1.find('.allBorder').addClass('displayNone');
                    DivStyleSec1.find('.singleBorder').removeClass('displayNone');
                }
            })

            DivStyleSec1.find('.vis-circular-slider-circle').mousedown(function (e) {
                mdown = true;
            }).mousemove(function (e) {
                if (mdown) {
                    var $slider = DivStyleSec1.find('.vis-circular-slider-dot')
                    var deg = getGradientDeg($slider, e);
                    $slider.css({ WebkitTransform: 'rotate(' + deg + 'deg)' });
                    $slider.css({ '-moz-transform': 'rotate(' + deg + 'deg)' });
                    $slider.attr("deg", deg);
                    applyCommend("gradient", deg);
                }
            });

            divTopNavigator.find("i").click(function () {
                var blok = DivViewBlock.find('.vis-active-block');
                var cmd = $(this).attr('command');
                isChange = true;
                var f = blok.closest('.fieldGroup').find('.fieldLbl');
                if (cmd == 'Show') {
                    if (blok.prop('tagName') == 'I') {
                        blok.attr("class", "fa fa-star");
                        blok.next().attr('showFieldIcon', false);
                    } else {
                        f.attr('showFieldText', false);
                        f.removeClass("displayNone");
                    }

                    divTopNavigator.find('[command="Hide"]').parent().show();
                    divTopNavigator.find('[command="Show"]').parent().hide();
                    divTopNavigator.hide();
                } else if (cmd == 'Hide') {
                    if (blok.prop('tagName') == 'I') {
                        blok.attr("class", "");
                        blok.next().attr('showFieldIcon', true);
                    } else if (blok.prop('tagName') == 'IMG') {
                        blok.css('display', 'none');
                    } else {
                        if (blok.hasClass('fieldValue')) {
                            blok.css("display", "none");
                        } else {
                            blok.attr('showFieldText', true);
                            blok.addClass("displayNone");
                        }
                        //blok.html('&nbsp;');
                    }

                    divTopNavigator.find('[command="Hide"]').parent().hide();
                    divTopNavigator.find('[command="Show"]').parent().show();
                    divTopNavigator.hide();
                    btnEditIcon.hide();
                } else if (cmd == 'SelectParent') {
                    isChange = false;
                    if (blok.parent().hasClass("fieldGroup")) {
                        blok.parent().parent().mousedown().mouseup();
                    } else {
                        blok.parent().mousedown().mouseup();
                    }

                } else if (cmd == 'Separate') {
                    applyunMerge(blok);
                    blok.find('.vis-split-cell:first').remove();
                    $(this).parent().hide();
                } else if (cmd == 'Merge') {
                    mergeCell();
                    divTopNavigator.find('[command="Merge"]').parent().hide();
                    mdown = false;
                } else if (cmd == 'Unlink') {
                    divTopNavigator.hide();
                    var fldLbl = blok.closest('.fieldGroup').find('.fieldLbl');
                    unlinkField(fldLbl.attr('title'), fldLbl);
                    btnEditIcon.hide();
                } else if (cmd == 'ShowImg') {
                    blok.closest('.fieldGroup').find('img').css("display", "unset");
                    divTopNavigator.find('[command="ShowImg"]').parent().hide();
                } else if (cmd == 'ShowValue') {
                    blok.closest('.fieldGroup').find('.fieldValue').css("display", "unset");
                    divTopNavigator.find('[command="ShowValue"]').parent().hide();
                }

                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
                // divTopNavigator.hide();
            });

            var viewBlock = DivViewBlock.find('.canvas *');

            viewBlock.mousedown(function (e) {
                if (e.target.tagName == 'SQL' || $(e.target).hasClass('fieldGroup')) {
                    return;
                }

                divTopNavigator.find('[command="Merge"]').parent().hide();
                divTopNavigator.find('[command="ShowImg"]').parent().hide();
                divTopNavigator.find('[command="ShowValue"]').parent().hide();
                DivStyleSec1.find('.imgTextCont').addClass('vis-disable-event');
                DivViewBlock.find('.vis-active-block').removeClass('vis-active-block');
                if (count == 1) {
                    DivViewBlock.find('.vis-viewBlock').addClass("vis-active-block");
                } else {
                    var secCount = $(e.target).closest('.vis-wizard-section').attr("sectionCount");
                    if (!DivGridSec.find("[sectionCount='" + secCount + "']").hasClass('section-active')) {
                        DivGridSec.find("[sectionCount='" + secCount + "'] .vis-grey-disp-el").click();
                    }

                    $(e.target).addClass("vis-active-block");

                    var top = e.target.offsetTop - divTopNavigator.outerHeight();
                    var left = e.target.offsetLeft;

                    divTopNavigator.css({
                        'top': top,
                        'left': left
                    });

                    divTopNavigator.show();
                    //divTopNavigator.find('[command="Unlink"]').parent().hide();
                    divTopNavigator.find('[command="fieldName"]').text('').hide();
                    if (e.target.tagName == 'IMG') {
                        btnEditIcon.show();
                        btnEditIcon.css({
                            "left": e.target.offsetLeft + e.target.width - 12,
                            "top": e.target.offsetTop,
                        });
                    } else {
                        btnEditIcon.hide();
                    }

                    if ($(e.target).closest('.fieldGroup').length > 0) {
                        divTopNavigator.find('[command="Unlink"]').parent().show();
                    } else {
                        divTopNavigator.find('[command="Unlink"]').parent().hide();
                    }

                    if ($(e.target).hasClass('fieldLbl')) {
                        divTopNavigator.find('[command="fieldName"]').text($(e.target).closest('.fieldGroup').find('.fieldLbl').attr('title')).show();
                        //divTopNavigator.find('[command="Unlink"]').parent().show();
                        var isTrue = $(e.target).attr('showFieldText') == 'true' ? true : false;
                        if (e.target.hasAttribute('showFieldText') && isTrue) {
                            divTopNavigator.find('[command="Hide"]').parent().hide();
                            divTopNavigator.find('[command="Show"]').parent().show();
                        } else {
                            divTopNavigator.find('[command="Show"]').parent().hide();
                            divTopNavigator.find('[command="Hide"]').parent().show();
                        }
                    } else if (e.target.tagName == 'I') {
                        var isTrue = $(e.target).next().attr('showFieldIcon') == 'true' ? true : false;
                        if ($(e.target).next().attr('showFieldIcon') && isTrue) {
                            divTopNavigator.find('[command="Hide"]').parent().hide();
                            divTopNavigator.find('[command="Show"]').parent().show();
                        } else {
                            divTopNavigator.find('[command="Show"]').parent().hide();
                            divTopNavigator.find('[command="Hide"]').parent().show();
                        }
                    }
                    else {
                        divTopNavigator.find('[command="Show"]').parent().hide();
                        divTopNavigator.find('[command="Hide"]').parent().hide();

                        if ($(e.target).closest('.fieldGroup').length > 0) {
                            var target = $(e.target).closest('.fieldGroup').find('.fieldLbl');                           
                            var trget = $(e.target).closest('.fieldGroup');
                            if (trget.find('img:hidden').length > 0) {
                                divTopNavigator.find('[command="ShowImg"]').parent().show();
                                divTopNavigator.find('[command="Hide"]').parent().hide();
                            } else if (trget.find('.fieldValue:hidden').length > 0) {
                                divTopNavigator.find('[command="Hide"]').parent().hide();
                                divTopNavigator.find('[command="ShowValue"]').parent().show();
                            } else if (trget.find('img').length == 1 && trget.find('.fieldValue').length == 1) {
                                divTopNavigator.find('[command="Hide"]').parent().show();
                                DivStyleSec1.find('.imgTextCont').removeClass('vis-disable-event');
                            } else {
                                DivStyleSec1.find('.imgTextCont').addClass('vis-disable-event');
                            }

                            var isTrue = target.attr('showFieldText') == 'true' ? true : false;
                            divTopNavigator.find('[command="fieldName"]').text(target.attr('title')).show();
                        }


                        if (isTrue) {
                            divTopNavigator.find('[command="Show"]').parent().show();
                        }

                    }

                    if ($(e.target).find('.vis-split-cell').length == 0) {
                        divTopNavigator.find('[command="Separate"]').parent().hide();
                        mdown = true;
                        var totalCol = DivGridSec.find('.colBox').length - 1;
                        activeSection.find('.grdDiv').each(function (e) {
                            var currentRow = Math.ceil((e + 1) / totalCol);
                            if ($(this).hasClass('vis-active-block')) {
                                startRowIndex = currentRow - 1;
                                startCellIndex = (e) - totalCol * (startRowIndex);
                            }
                        });
                    } else {
                        divTopNavigator.find('[command="Separate"]').parent().show();
                    }
                }

                if ($(e.target).hasClass('vis-wizard-section')) {
                    divTopNavigator.find('[command="fieldName"]').text('Section ' + $(e.target).attr('sectioncount')).show();
                }

                if ($(e.target).hasClass('vis-viewBlock')) {
                    divTopNavigator.find('[command="fieldName"]').text('Main container').show();
                }
                //$(e.target).not('.ui-resizable-handle').addClass("vis-active-block");
                //$(this).resizable();
            }).mouseup(function (e) {
                fill($(e.target));
            });

            DivStyleSec1.find('[data-command]').on('change', function (e) {
                $(this).removeClass('vis-editor-validate');
                var commend = $(this).data('command');
                var styleValue = $(this).val();
                var mtext = styleValue.replace(/\d+/g, "").replace('.', '');
                var mvalue = styleValue.replace(styleValue.replace(/\d+/g, ""), "");
                if (editorProp[commend] && editorProp[commend].measurment && styleValue != "" && $(this).attr('type') != 'color') {
                    if (measurment.indexOf(mtext) < 0) {
                        if (isNaN(Number(mvalue))) {
                            $(this).addClass('vis-editor-validate');
                            return;
                        }
                        $(this).val(mvalue + "px");
                    } else if (isNaN(Number(mvalue))) {
                        $(this).addClass('vis-editor-validate');
                        return;
                    }
                }
               

                if (commend == 'backgroundColor') {
                    // var clr= rgb2hex(styleValue);
                    DivStyleSec1.find('.vis-zero-BTopLeftBLeft:first').css('background-color', styleValue);
                    DivStyleSec1.find('[data-command="backgroundColor"]').val(styleValue);
                } else if (commend == 'color') {
                    DivStyleSec1.find('.vis-zero-BTopLeftBLeft:last').css('background-color', styleValue);
                    DivStyleSec1.find('[data-command="color"]').val(styleValue);
                }

                applyCommend(commend, $(this).val());
            });

            DivStyleSec1.find('[data-command1]').on('click', function (e) {

                var commend = $(this).data('command1');
                var tag = activeSection.find('.vis-active-block');
                var isStyleExist = false;
                if (editorProp[commend].measurment) {
                    isStyleExist = checkStyle(editorProp[commend].proprty, editorProp[commend].value, tag)
                } else {
                    isStyleExist = checkStyle(editorProp[commend].proprty, false, tag)
                }
                

                if ((editorProp[commend].proprty == "justify-content" || editorProp[commend].proprty == "align-items") && checkStyle("display", "flex", tag)) {
                    //applyCommend("displayFlex", "");
                    tag[0].style.removeProperty("display");                    
                }
                if (isStyleExist) {
                    applyCommend(commend, "");
                } else {
                    applyCommend(commend, editorProp[commend].value);
                }

                if ($(this).parent().hasClass('vis-hr-elm-inn-active')) {
                    $(this).parent().removeClass('vis-hr-elm-inn-active');
                } else {
                    $(this).parent().addClass('vis-hr-elm-inn-active');
                }
            });

            DivStyleSec1.find('[data-command2]').on('click', function (e) {
                divTopNavigator.hide();
                var tag = activeSection.find('.vis-active-block').closest('.fieldGroup');
                var commend = $(this).data('command2');
                var styleProp = tag.find('.fieldValue').attr('style');
                var classPro = tag.find('.fieldValue').attr('class');
                tag.find('.fieldValue br').remove();
                if (commend == 'SwapImgTxt') {
                    tag.find('img').before(tag.find('.fieldValue'));
                } else if (commend == 'SwapTxtImg') {
                    tag.find('.fieldValue').before(tag.find('img'));
                } else if (commend == 'SwapTxtImgBr') {
                    tag.find('.fieldValue').before(tag.find('img'));
                    tag.find('.fieldValue').prepend('<br>');
                }
                else if (commend == 'SwapImgTxtBr') {
                    tag.find('img').before(tag.find('.fieldValue'));
                    tag.find('.fieldValue').append('<br>');
                }
            });

            DivGridSec.find('.addGridRow').click(function () {
                var rClone = DivGridSec.find('.rowBox:first').clone(true);
                rClone.show();
                DivGridSec.find('.DivRowBox').append(rClone);
                createGrid();
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.addGridCol').click(function () {
                var cClone = DivGridSec.find('.colBox:first').clone(true);
                cClone.show();
                DivGridSec.find('.DivColBox').append(cClone);
                createGrid();
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.addGridSection').click(function () {
                isNewSection = true;
                DivGridSec.find('.section-active').removeClass('section-active');
                var sClone = DivGridSec.find('.grid-Section:first').clone(true);
                sClone.removeClass('displayNone');
                sClone.addClass('section-active');
                sectionCount = Number(DivGridSec.find('.grid-Section:last').attr("sectionCount")) + 1;
                sClone.attr('sectionCount', sectionCount);
                sClone.find('.vis-grey-disp-ttl').text("Section " + sectionCount);
                DivGridSec.find('.vis-sectionAdd').append(sClone);
                activeSection = $('<div sectionCount="' + sectionCount + '"  class="section' + sectionCount + ' vis-wizard-section"></div>')
                DivViewBlock.find('.vis-viewBlock').append(activeSection);
                //sectionCount++;
                DivGridSec.find('.rowBox:not(:first)').remove();
                DivGridSec.find('.colBox:not(:first)').remove();
                DivGridSec.find('.addGridRow').click();
                DivGridSec.find('.addGridCol').click();

            });

            DivGridSec.find('.grid-Section .vis-grey-disp-el-xross').click(function () {
                var secNo = $(this).closest('.grid-Section').attr('sectionCount');
                if ($(this).closest('.grid-Section').hasClass('section-active')) {
                    DivGridSec.find('.grid-Section').eq(1).addClass('section-active');
                    DivGridSec.find('.grid-Section').eq(1).find('.vis-grey-disp-el').click();
                }
                $(this).closest('.grid-Section').remove();
                var removeSection = DivViewBlock.find('.vis-viewBlock .section' + secNo);

                removeSection.find('.fieldLbl').each(function () {
                    if ($(this).attr('title') && $(this).attr('title').length > 0) {
                        unlinkField($(this).attr('title'), $(this));
                    }
                });

                removeSection.remove();
                secNo = DivGridSec.find('.section-active').attr('sectionCount');
                activeSection = DivViewBlock.find('.vis-viewBlock .section' + secNo);
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.grid-Section .vis-grey-disp-el').click(function () {
                DivGridSec.find('.section-active').removeClass('section-active');
                $(this).parent().addClass('section-active');
                var secNo = DivGridSec.find('.section-active').attr('sectionCount');

                activeSection = DivViewBlock.find('.vis-viewBlock .section' + secNo);
                DivViewBlock.find('.vis-active-block').removeClass('vis-active-block');
                activeSection.addClass('vis-active-block');
                createGridRowCol();
            });

            DivGridSec.find('.grdRowDel').click(function () {
                var idx = $(this).closest('.rowBox').index();
                rowIdx = idx;
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1;
                var dNo = (idx * totalCol + 1) - totalCol;
                for (var i = dNo; i < (dNo + totalCol); i++) {
                    activeSection.find('.grdDiv').eq(i - 1).addClass('del');
                    var blk = activeSection.find('.grdDiv').eq(i - 1).find('.fieldLbl');
                    if (blk && blk.attr('title') && blk.attr('title').length > 0) {
                        unlinkField(blk.attr('title'), blk);
                    }
                }
                activeSection.find('.del').remove();
                $(this).closest('.rowBox').remove();
                gridCss(-1, 0);
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.grdRowAdd').click(function () {
                var idx = $(this).closest('.rowBox').index() - 1;
                var rClone = DivGridSec.find('.rowBox:first').clone(true);
                rClone.show();
                DivGridSec.find('.DivRowBox').append(rClone);

                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1;


                var pos = ((idx + 1) * totalCol);
                rowIdx = (idx + 1);
                for (var i = 0; i < totalCol; i++) {
                    activeSection.find('.grdDiv').eq(pos - 1).after("<div class='grdDiv' style='padding:10px;'></div>");
                }

                gridCss(1, 0);

            });

            DivGridSec.find('.grdColDel').click(function () {
                var idx = $(this).closest('.colBox').index() - 1;
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1;
                colIdx = (idx + 1);
                for (var i = 0; i < totalRow; i++) {
                    for (var j = 0; j < totalCol; j++) {
                        if (j == idx) {
                            var dNo = totalCol * i + j;
                            activeSection.find('.grdDiv').eq(dNo).addClass('del');
                            var blk = activeSection.find('.grdDiv').eq(dNo).find('.fieldLbl');
                            if (blk && blk.attr('title') && blk.attr('title').length > 0) {
                                unlinkField(blk.attr('title'), blk);
                            }
                        }
                    }
                }

                activeSection.find('.del').remove();
                $(this).closest('.colBox').remove();
                gridCss(0, -1);
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.grdColAdd').click(function () {

                var cClone = DivGridSec.find('.colBox:first').clone(true);
                cClone.show();
                DivGridSec.find('.DivColBox').append(cClone);

                var idx = $(this).closest('.colBox').index() - 1;
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1;
                colIdx = idx + 1;
                for (var i = 0; i < totalRow; i++) {
                    for (var j = 0; j < totalCol; j++) {
                        if (j == idx) {
                            var pos = totalCol * i + j;
                            addedColPos.push(pos + 1);
                            activeSection.find('.grdDiv').eq(pos).after("<div class='grdDiv' style='padding:10px;'></div>");
                        }
                    }
                }
                gridCss(0, 1);

            });

            DivGridSec.find('.mergeCell').click(function () {
                mergeCell();
            });

            txtColGap.change(function () {
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
                gridCss();
            });

            txtRowGap.change(function () {
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
                gridCss();
            });

            DivGridSec.find('.rowBox input,select').change(function () {
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
                gridCss();
            });

            DivGridSec.find('.colBox input,select').change(function () {
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
                gridCss();
            });

            //DivCardField.find('.vis-grey-icon .fa-circle').click(function () {
            //    if ($(this).hasClass('vis-succes-clr')) {
            //        var fid = $(this).closest('.fieldLbl').attr('fieldid');
            //        DivViewBlock.find('[fieldid="' + fid + '"]').mousedown().mouseup();
            //        setTimeout(function () {
            //            divTopNavigator.find('[command="Unlink"]').click();
            //        }, 100);

            //    } else {
            //        linkField($(this).closest('.fieldLbl'));
            //    }
            //});

            btn_BlockCancel.click(function () {
                DivTemplate.find('.mainTemplate[templateid="' + AD_HeaderLayout_ID + '"]').parent().click();
                if (count > 1) {
                    addSelectedTemplate();
                    fillcardLayoutfromTemplate();
                }

                btn_BlockCancel.hide();
            });



        };

        var getGradientDeg = function (id, e) {
            var radius = 9;
            var deg = 0;
            var elP = id.parent().offset();
            var elPos = { x: elP.left, y: elP.top };

            if (mdown) {
                var mPos = { x: e.clientX - elPos.x, y: e.clientY - elPos.y };
                var atan = Math.atan2(mPos.x - radius, mPos.y - radius);
                deg = -atan / (Math.PI / 180) + 180; // final (0-360 positive) degrees from mouse position 
                deg = Math.ceil(deg);
                return deg;
                // AND FINALLY apply exact degrees to ball rotation

            }
        }

        function addSelectedTemplate() {
            var $this = DivTemplate.find('.vis-active-template').clone(true);
            if ($this.attr("lastupdated")) {
                spnLastSaved.text(VIS.Msg.getMsg("LastSaved") + " " + $this.attr("lastupdated"));
            }
            //$this.find('.grdDiv').html('');
            $this.find('.mainTemplate').css("zoom",1);
            CardCreatedby = $this.attr("createdBy");
            isSystemTemplate = $this.attr("isSystemTemplate");
            AD_HeaderLayout_ID = $this.find('.mainTemplate').attr('templateid');
            templateName = $this.find('.mainTemplate').attr('name');
            if (AD_HeaderLayout_ID == "0") {

                $this.find('.mainTemplate').html($('<div sectionCount="1" class="section1 vis-wizard-section" style="padding:5px;"></div>'));
            }
            $this.find('[contenteditable]').attr('contenteditable', true);
            txtTemplateName.val($this.find('.mainTemplate').attr('name'));
            AD_HeaderLayout_ID = $this.find('.mainTemplate').attr('templateid');
            if ($this.html() != "" || $this.html() != null) {
                $this.find('.vis-wizard-section').each(function () {
                    var row = $(this).attr('row');
                    var col = $(this).attr('col');
                    var arr = [];
                    for (var i = 0; i < (row * col); i++) {
                        arr.push('<div class="grdDiv" style="display:none"></div>');
                    }

                    $(this).find('.grdDiv').each(function () {
                        var areagrid = $(this).css('grid-area').split('/');
                        var idx = col * ($.trim(areagrid[0]) - 1) + ($.trim(areagrid[1]) - 1);
                        if ($.trim(areagrid[0]) != ($.trim(areagrid[2]) - 1) || $.trim(areagrid[1]) != ($.trim(areagrid[3]) - 1)) {
                            $(this).append('<span class="vis-split-cell"></span>');
                        }
                        arr[idx] = $(this)[0].outerHTML;
                    });
                    $(this).html(arr.join(" "));
                });
                DivViewBlock.find('.grdDiv').unbind('mouseover');
                DivViewBlock.find('.vis-viewBlock').attr("style", $this.find('.mainTemplate').attr('style') || '');
                DivViewBlock.find('.vis-viewBlock').html($this.find('.mainTemplate').html());
                DivViewBlock.find('.grdDiv').mouseover(function (e) {
                    if (mdown && ($(this).find('.vis-split-cell').length == 0)) {
                        selectTo($(this));
                    }
                });
            }

        }

        var fillcardLayoutfromTemplate = function () {
            gridObj = {};
            var sClone = DivGridSec.find('.grid-Section:first').clone(true);
            sClone.removeClass('displayNone');
            DivGridSec.find('.grid-Section:not(:first)').remove();
            DivViewBlock.find('.vis-wizard-section').each(function (i) {
                var secNo = $(this).attr('sectionCount');
                if (i == 0) {
                    sClone.addClass('section-active');
                    $(this).addClass('vis-active-block');
                }
                sClone.attr('sectionCount', secNo);
                sClone.find('.vis-grey-disp-ttl').text("Section " + secNo);
                DivGridSec.find('.vis-sectionAdd').append(sClone);
                sClone = DivGridSec.find('.grid-Section:first').clone(true);
                sClone.removeClass('displayNone');

                if (!gridObj["section" + secNo]) {
                    var totalRow = $(this).attr('row');
                    var totalCol = $(this).attr('col');
                    var Obj = {};
                    for (var i = 0; i < totalRow; i++) {
                        Obj['row_' + i] = {
                            val: 1,
                            msr: 'auto'
                        }
                    };

                    for (var j = 0; j < totalCol; j++) {
                        Obj['col_' + j] = {
                            val: 1,
                            msr: 'auto'
                        }
                    }

                    Obj["sectionNo"] = secNo;
                    Obj["rowGap"] = 0;
                    Obj["colGap"] = 0;
                    Obj["totalRow"] = totalRow;
                    Obj["totalCol"] = totalCol;

                    gridObj["section" + secNo] = Obj;
                }
            });

            DivGridSec.find('.section-active .vis-grey-disp-el').click();
            DivGridSec.find('.vis-grey-disp-el-xross').eq(1).hide();
         
        }

        function gridCss(r, c) {
            if (!r) {
                r = 0;
            }

            if (!c) {
                c = 0;
            }

            var secNo = DivGridSec.find('.section-active').attr('sectionCount');
            var Obj = {
            }
            var rowCss = "";
            DivGridSec.find('.rowBox:not(:first)').each(function (i) {
                if (i == 0) {
                    $(this).find('.grdRowDel').hide();
                }
                if ($(this).find('select :selected').val() == 'auto') {
                    rowCss += $(this).find('select :selected').val() + ' ';
                } else {
                    rowCss += $(this).find('input').val() + '' + $(this).find('select :selected').val() + ' ';
                }

                Obj['row_' + i] = {
                    val: $(this).find('input').val(),
                    msr: $(this).find('select :selected').val(),
                }
            });
            var colCss = "";
            DivGridSec.find('.colBox:not(:first)').each(function (i) {
                if (i == 0) {
                    $(this).find('.grdColDel').hide();
                }

                if ($(this).find('select :selected').val() == 'auto') {
                    colCss += $(this).find('select :selected').val() + ' ';
                } else {
                    colCss += $(this).find('input').val() + '' + $(this).find('select :selected').val() + ' ';
                }
                Obj['col_' + i] = {
                    val: $(this).find('input').val(),
                    msr: $(this).find('select :selected').val()
                }
            });

            var totalRow = DivGridSec.find('.rowBox:not(:first)').length;
            var totalCol = DivGridSec.find('.colBox:not(:first)').length;
            Obj["sectionNo"] = secNo;
            Obj["rowGap"] = txtRowGap.val();
            Obj["colGap"] = txtColGap.val();
            Obj["totalRow"] = totalRow;
            Obj["totalCol"] = totalCol;

            gridObj["section" + secNo] = Obj;
            var grSec = activeSection;
            grSec.attr("row", totalRow);
            grSec.attr("col", totalCol);

            grSec.css({
                'grid-template-columns': colCss,
                'grid-template-rows': rowCss,
                'gap': txtRowGap.val() + 'px ' + txtColGap.val() + 'px'
            });

            var isEnter = false;
            grSec.find('.grdDiv').each(function (index) {
                var gArea = $(this).css('grid-area').split('/');
                var r_start = Number($.trim(gArea[0]));
                var r_end = Number($.trim(gArea[2]));
                var c_start = Number($.trim(gArea[1]));
                var c_end = Number($.trim(gArea[3]));
                if (rowIdx != null) {
                    if ((rowIdx > (r_start - 1) && rowIdx < (r_end - 1))) {
                        $(this).css('grid-area', r_start + '/' + c_start + '/' + (r_end + r) + '/' + (c_end));
                        isEnter = true;
                    }

                    if (isEnter && r > 0) {
                        for (var i = Number(r_start); i <= (Number(r_end)); i++) {
                            for (var j = c_start; j < c_end; j++) {
                                var pos = totalCol * i + j;
                                grSec.find('.grdDiv').eq(pos - 1).css('display', 'none');
                            }
                            if (rowIdx == i) {
                                break;
                            }
                        }
                        isEnter = false;
                        rowIdx = null;
                    }

                } else if (colIdx != null) {
                    if ((colIdx > (c_start - 1) && colIdx < (c_end - 1))) {
                        $(this).css('grid-area', r_start + '/' + c_start + '/' + r_end + '/' + (c_end + c));
                        isEnter = true;
                    }

                    if (isEnter && c > 0) {
                        for (var i = 0; i < addedColPos.length; i++) {
                            var rowPos = Math.floor(addedColPos[i] / totalCol) + 1;
                            if (rowPos > (r_start - 1) && rowPos < (r_end)) {
                                grSec.find('.grdDiv').eq(addedColPos[i]).css('display', 'none');
                            }
                        }

                        isEnter = false;
                        colIdx = null;
                        addedColPos = [];
                    }
                }

                if ($(this).find('.vis-split-cell').length == 0) {
                    var rowPosition = (Math.floor(index / totalCol)) + 1;
                    var colposition = (index % totalCol) + 1;
                    $(this).css('grid-area', rowPosition + '/' + colposition + '/' + (rowPosition + 1) + '/' + (colposition + 1));
                }
            });

        }

        function createGrid() {
            var rowBox = DivGridSec.find('.rowBox');
            var colBox = DivGridSec.find('.colBox');
            var totalRow = rowBox.length - 1;
            var totalCol = colBox.length - 1;
            var grSec = activeSection;

            if (totalCol > 0 && totalRow > 0) {
                var oldrow = grSec.attr('row');
                var oldcol = grSec.attr('col');
                if (oldcol != totalCol && !isNewSection) {
                    for (var r = 1; r <= oldrow; r++) {
                        var pos = (r * oldcol) + (r - 1);
                        grSec.find('.grdDiv').eq(pos - 1).after("<div class='grdDiv' style='padding:10px;'></div>");
                    }

                } else {
                    var totalDiv = totalRow * totalCol - grSec.find('.grdDiv').length;
                    for (var i = 0; i < totalDiv; i++) {
                        grSec.append("<div class='grdDiv' style='padding:10px;'></div>");
                    }
                }

                grSec.find('.grdDiv').unbind('mouseover');
                grSec.find('.grdDiv').mouseover(function (e) {
                    if (mdown && ($(this).find('.vis-split-cell').length == 0)) {
                        selectTo($(this));
                    }

                });
                isNewSection = false;
            }

            gridCss();

        }

        function createGridRowCol() {
            DivGridSec.find('.rowBox:not(:first)').remove();
            var secNo = DivGridSec.find('.section-active').attr('sectionCount');
            var section = gridObj["section" + secNo];
            var rClone = DivGridSec.find('.rowBox:first').clone(true);
            rClone.show();
            DivGridSec.find('.colBox:not(:first)').remove();
            var cClone = DivGridSec.find('.colBox:first').clone(true);
            cClone.show();
            if (section) {
                for (let key in section) {
                    var item = section[key];
                    if (key.indexOf('row_') != -1) {
                        rClone.find('input').val(item.val);
                        rClone.find('select').val(item.msr);
                        DivGridSec.find('.DivRowBox').append(rClone);
                        rClone = DivGridSec.find('.rowBox:last').clone(true);
                    } else if (key.indexOf('col_') != -1) {
                        cClone.find('input').val(item.val);
                        cClone.find('select').val(item.msr);
                        DivGridSec.find('.DivColBox').append(cClone);
                        cClone = DivGridSec.find('.colBox:last').clone(true);
                    }
                }
                txtRowGap.val(section.rowGap);
                txtColGap.val(section.colGap);
                DivGridSec.find('.rowBox .grdRowDel').eq(1).hide();
                DivGridSec.find('.colBox .grdColDel').eq(1).hide();
            }

        }

        function selectTo(cell) {
            var totalCol = DivGridSec.find('.colBox').length - 1;
            var idx = activeSection.find(cell).index();
            if (idx < 0) {
                return;
            }

            var currentRow = Math.ceil((idx + 1) / totalCol) - 1;

            //var row = cell.parent();
            var cellIndex = (idx) - totalCol * (currentRow);
            var rowIndex = currentRow;
            var rowStart, rowEnd, cellStart, cellEnd;

            if (rowIndex < startRowIndex) {
                rowStart = rowIndex;
                rowEnd = startRowIndex;
            } else {
                rowStart = startRowIndex;
                rowEnd = rowIndex;
            }

            if (cellIndex < startCellIndex) {
                cellStart = cellIndex;
                cellEnd = startCellIndex;
            } else {
                cellStart = startCellIndex;
                cellEnd = cellIndex;
            }

            //console.log(rowStart, rowEnd, cellStart, cellEnd);
            DivViewBlock.find('.vis-active-block').removeClass('vis-active-block');
            for (var i = rowStart; i <= rowEnd; i++) {
                for (var j = cellStart; j <= cellEnd; j++) {
                    activeSection.find('.grdDiv').eq(totalCol * i + j).addClass("vis-active-block");
                }
            }

            if (DivViewBlock.find('.vis-active-block').length > 1) {
                divTopNavigator.find('[command="Merge"]').parent().show();
            } else {
                divTopNavigator.find('[command="Merge"]').parent().hide();
            }
        }

        function mergeCell() {
            var rowStart = 0, rowEnd = 0, colStart = 0, colEnd = 1;
            var rowCount = 0;
            var colCount = 0;
            var countActive = 1;
            var totalActive = activeSection.find('.vis-active-block').length;
            var lastRow = 1;
            var isRowChange = false;
            var activColInRow = 0;
            var colBoxLen = DivGridSec.find('.colBox').length - 1;
            activeSection.find('.grdDiv').each(function (e) {
                colCount++;
                var currentRow = Math.ceil((e + 1) / colBoxLen);
                if (currentRow != lastRow) {
                    colCount = 1;
                }
                lastRow = currentRow;
                //$(this).css('grid-area', currentRow + '/' + colCount + '/' + (currentRow + 1) + '/' + (colCount+1));

                if ($(this).hasClass('vis-active-block')) {
                    if (activColInRow == 0) {
                        activColInRow = currentRow;
                    }
                    if (rowStart == 0 && colStart == 0) {
                        rowStart = currentRow;
                        colStart = colCount;
                        colEnd = colCount;
                    }

                    if (activColInRow == currentRow) {
                        colEnd++;
                    }
                    if (countActive == totalActive) {
                        rowEnd = currentRow + 1;
                    }
                    countActive++;
                    $(this)[0].style.removeProperty('grid-area');
                    $(this)[0].style.removeProperty('display');
                }

            });

            var unMearge = $('<span class="vis-split-cell"></span>');
            activeSection.find('.vis-active-block:not(:first)').hide().removeClass('vis-active-block');
            activeSection.find('.vis-active-block:first').css('grid-area', rowStart + '/' + colStart + '/' + rowEnd + '/' + colEnd).append(unMearge);

            unMearge.click(function () {
                applyunMerge($(this).parent());
                $(this).remove();
            });
        }

        function applyunMerge(e) {
            var gArea = e.css('grid-area').split('/');
            var totalCol = DivGridSec.find('.colBox').length - 1;
            var DVB = activeSection.find('.grdDiv');
            for (var i = Number($.trim(gArea[0])); i < Number($.trim(gArea[2])); i++) {
                for (var j = Number($.trim(gArea[1])); j < Number($.trim(gArea[3])); j++) {
                    var gIdx = totalCol * (i - 1) + (j - 1);
                    DVB.eq(gIdx)[0].style.removeProperty('grid-area');
                    DVB.eq(gIdx)[0].style.removeProperty('display');
                }
            }
            gridCss();
        }

        function checkStyle(prop, val, htm) {
            var styles = htm.attr('style'),
                value = false;

            styles && styles.split(";").forEach(function (e) {
                var style = e.split(":");
                if (val && $.trim(style[0]) === prop && $.trim(style[1]) === val) {
                    value = true;
                } else if (!val && $.trim(style[0]) === prop) {
                    value = true;
                }
            });
            return value;
        }

        function applyCommend(commend, styleValue) {
            isChange = true;
            if (isChange && AD_HeaderLayout_ID != "0") {
                btn_BlockCancel.show();
            }
            var tag = DivViewBlock.find('.vis-active-block');

            if (editorProp[commend].proprty == "flex-direction") {
                tag[0].style.removeProperty("display");
                tag.find('.fieldGroup').removeAttr('style');
            }

            if (commend != 'gradient' && (styleValue == "" || styleValue == null)) {
                tag[0].style.removeProperty(editorProp[commend].proprty);
                return;
            }           


            if (commend == 'gradient') {
                var color1 = DivStyleSec1.find('.' + commend + '1').val();
                var color2 = DivStyleSec1.find('.' + commend + '2').val();
                var prcnt = DivStyleSec1.find('.vis-percentage-slidr').val();
                var deg = DivStyleSec1.find('.vis-circular-slider-dot').attr('deg');
                styleValue = 'linear-gradient(' + deg + 'deg,' + color1 + ' ' + prcnt + '%,  ' + color2 + ')';
                DivStyleSec1.find('.vis-gradient-comp').css('background', styleValue);
                DivStyleSec1.find('[data-command="gradientInput"]').val('(' + deg + 'deg,' + color1 + ' ' + prcnt + '%,  ' + color2 + ')');
            }

            if (commend == 'gradientInput') {
                styleValue = 'linear-gradient' + styleValue;
            }

            if (commend == 'boxShadow') {
                var x = DivStyleSec1.find('.boxX').val();
                var y = DivStyleSec1.find('.boxY').val();
                var b = DivStyleSec1.find('.boxB').val();
                var c = DivStyleSec1.find('.boxC').val();
                styleValue = x + ' ' + y + ' ' + b + ' ' + c;
            }

            if (editorProp[commend].proprty == 'justify-content' || editorProp[commend].proprty == "align-items" || editorProp[commend].proprty == "flex-direction") {
                tag.css("display", "flex");
                if (editorProp[commend].proprty == "flex-direction") {
                    tag.find('.fieldGroup').css({
                        "display": "flex",
                        "flex-direction": $.trim(styleValue)
                    });
                }
            }

            tag.css(editorProp[commend].proprty, $.trim(styleValue));
        }

        function fill(htm) {
            DivStyleSec1.find('#master001_' + WindowNo + ' input').val('');
            DivStyleSec1.find('#master001_' + WindowNo + ' select').val('');
            DivStyleSec1.find('.gradient1').val('#833ab4');
            DivStyleSec1.find('.gradient2').val('#fcb045');
            DivStyleSec1.find("[data-command1]").parent().removeClass('vis-hr-elm-inn-active');
            var styles = htm.attr('style');
            if (htm.find('sql').length > 0) {
                txtSQLQuery.val(VIS.secureEngine.decrypt(htm.attr("query")));
            } else {
                txtSQLQuery.val('');
            }

            //styles = styles.split(';');
            //styles.join("\n\r");
            txtCustomStyle.val(styles);
            styles && styles.split(";").forEach(function (e) {
                var style = e.split(":");
                for (const a in editorProp) {
                    if ($.trim(style[0]) == $.trim(editorProp[a].proprty)) {
                        var v = $.trim(style[1]);
                        if (editorProp[a].value == '') {
                            DivStyleSec1.find("[data-command='" + a + "']").val(v);
                            if (a == 'backgroundColor') {
                                DivStyleSec1.find('.vis-zero-BTopLeftBLeft:first').css('background-color', v);
                                DivStyleSec1.find("[data-command='" + a + "'][type='color']").val(rgb2hex(v));
                            } else if (a == 'color') {
                                DivStyleSec1.find('.vis-zero-BTopLeftBLeft:last').css('background-color', v);
                                DivStyleSec1.find("[data-command='" + a + "'][type='color']").val(rgb2hex(v));
                            }
                        } else {
                            DivStyleSec1.find("[data-command1='" + a + "']").parent().addClass('vis-hr-elm-inn-active');
                        }
                        break;
                    }
                }
            });
        }

        function rgb2hex(rgb) {
            if (/^#[0-9A-F]{6}$/i.test(rgb)) return rgb;

            rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
            function hex(x) {
                return ("0" + parseInt(x).toString(16)).slice(-2);
            }
            return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);

        }

        function unlinkField(fieldName, itm) {
            itm.closest('.fieldGroup').remove();            
            itm.remove();
            divTopNavigator.hide();
        }
       
        function getTemplateDesign() {
            var url = VIS.Application.contextUrl + "CardView/getSystemTemplateDesign";
            DivTemplate.find('.vis-cardSingleViewTemplate:not(:first)').remove();
            var obj = {
            }
            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(obj),
                success: function (data) {
                    var result = JSON.parse(data);
                    DivTemplate.find('.vis-cardTemplateContainer').append(result);
                    //IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    //IsBusy(false);
                }, complete: function () {
                    DivTemplate.find('.vis-cardSingleViewTemplate').click(function () {
                        DivTemplate.find('.vis-cardSingleViewTemplate').removeClass('vis-active-template');
                        $(this).addClass('vis-active-template');
                    });

                    scaleTemplate();                   
                }
            });
        }



        function saveTemplate(e) {
           
            if (txtTemplateName.val() == "") {
                VIS.ADialog.error("FillMandatory", true, "Template Name");
                return false;
            }

            //IsBusy(true);
            var fieldObj = [];
            var seq = 10;
            var cardSection = [];

            cardViewColArray = [];
            DivViewBlock.find('.grdDiv:not(:hidden)').each(function (index) {
                var gridArea = $(this).css('grid-area').split('/');
                var secNo = $(this).closest('.vis-wizard-section').attr("sectioncount");
                gridObj['section' + secNo]["style"] = $(this).closest('.vis-wizard-section').attr("style");
                gridObj['section' + secNo]["sectionID"] = $(this).closest('.vis-wizard-section').attr("sectionid") || 0;
                if ($(this).find('.fieldGroup:not(:hidden)').length > 0) {
                    $(this).find('.fieldGroup:not(:hidden)').each(function (index) {                        
                        var contentValue = "";
                        var contentLable = $(this).find('.fieldLbl').text();
                        var valueStyle = "";
                        if ($(this).find('img').length > 0 && $(this).find('.fieldValue').length > 0) {
                            contentValue = '<img src="' + $(this).find('img').attr('src') + '" style="' + $(this).find('img').attr('style') + '">';
                            contentValue += ' |'+$(this).find('.fieldValue').text();
                            var isBR = $(this).find('br').length;
                            if ($(this).find('img').next('.fieldValue').length > 0) {
                                valueStyle = '@img::' + $(this).find('img').attr('style') || '';
                                if (isBR > 0) {
                                    valueStyle += '|<br>';
                                }
                                valueStyle += ' |@value::' + $(this).find('.fieldValue').attr('style') || 'display:none';
                            } else {
                                valueStyle = '@value::' + $(this).find('.fieldValue').attr('style') || 'display:none';
                                if (isBR > 0) {
                                    valueStyle += '|<br>';
                                }
                                valueStyle += ' |@img::' + $(this).find('img').attr('style') || '';
                            }


                        } else if ($(this).find('img').length > 0) {
                            contentValue = '<img src="' + $(this).find('img').attr('src') + '" style="' + $(this).find('img').attr('style') + '">';
                            valueStyle = '@value::' + $(this).find('.fieldValue').attr('style')||'';
                            valueStyle += ' |@img::' + $(this).find('img').attr('style') || '';
                        } else {
                            contentValue = $(this).find('.fieldValue').text();
                            valueStyle = $(this).find('.fieldValue').attr('style') || '';
                        }

                        var columnSQL = null;
                        if ($(this).find('sql').length > 0) {
                            columnSQL = $(this).attr('query') || null;
                        }
                        var hideFieldIcon = true;
                        if ($(this).find('.fa-star').length == 0) {
                            hideFieldIcon = true;
                        }
                        if ($(this).find('.fieldLbl').attr('showfieldicon')) {
                            hideFieldIcon = $(this).find('.fieldLbl').attr('showfieldicon') == 'true' ? true : false;
                        }

                        obj1 = {
                            cardFieldID: $(this).attr('cardfieldid'),
                            sectionNo: secNo * 10,
                            rowStart: $.trim(gridArea[0]),
                            rowEnd: $.trim(gridArea[2]),
                            colStart: $.trim(gridArea[1]),
                            colEnd: $.trim(gridArea[3]),
                            seq: seq,
                            style: $(this).closest('.grdDiv').attr('style'),
                            fieldID: $(this).find('.fieldLbl').attr('fieldid'),
                            valueStyle: valueStyle,
                            fieldStyle: $(this).find('.fieldLbl').attr('style') || '',
                            hideFieldIcon: hideFieldIcon,
                            hideFieldText: $(this).find('.fieldLbl').attr('showfieldtext') == 'true' ? true : false,
                            columnSQL: columnSQL,
                            contentFieldValue: contentValue,
                            contentFieldLable: contentLable
                        }

                        //var f = {}
                        //f.AD_Field_ID = obj1.fieldID;
                        //f.CardViewID = 0;
                        //cardViewColArray.push(f);
                       
                        fieldObj.push(obj1);
                    });
                } else {
                    var obj1 = {
                        cardFieldID: null,
                        sectionNo: secNo * 10,
                        rowStart: $.trim(gridArea[0]),
                        rowEnd: $.trim(gridArea[2]),
                        colStart: $.trim(gridArea[1]),
                        colEnd: $.trim(gridArea[3]),
                        seq: seq,
                        style: $(this).attr('style'),
                        fieldID: null,
                        valueStyle: "",
                        fieldStyle: '',
                        hideFieldIcon: false,
                        hideFieldText: false,
                        columnSQL: "",
                        contentFieldValue: null,
                        contentFieldLable: null
                    }

                   
                    fieldObj.push(obj1);
                }
                seq += 10;
            });
            
            Object.keys(gridObj).forEach(function (key) {
                var fobj = {
                    sectionName: 'section ' + gridObj[key].sectionNo,
                    sectionID: gridObj[key].sectionID,
                    sectionNo: gridObj[key].sectionNo * 10,
                    style: gridObj[key].style,
                    totalRow: gridObj[key].totalRow,
                    totalCol: gridObj[key].totalCol,
                    rowGap: gridObj[key].rowGap,
                    colGap: gridObj[key].colGap
                };
                cardSection.push(fobj);
            });

            var finalobj = {
                CardViewID: 0,
                templateID: AD_HeaderLayout_ID || 0,
                templateName: txtTemplateName.val(),
                style: DivViewBlock.find('.vis-viewBlock').attr('style'),
                cardSection: cardSection,
                cardTempField: fieldObj,
                isSystemTemplate:'Y'
            }

            var url = VIS.Application.contextUrl + "CardView/saveCardTemplate";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(finalobj),
                success: function (data) {
                    var result = JSON.parse(data);
                    AD_HeaderLayout_ID = result;
                    isSystemTemplate = 'Y';         
                    toastr.success(VIS.Msg.getMsg('SavedSuccessfully'), '', { timeOut: 3000, "positionClass": "toast-top-center", "closeButton": true, });
                    getTemplateDesign();
                   // IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                   // IsBusy(false);
                }
            });

        }



        this.getRoot = function () {
            return root;
        };

        function scaleTemplate() {
            DivTemplate.find('.vis-cardSingleViewTemplate').each(function () {
                var pH = $(this).height();
                var pW = $(this).width();
                var inner = $(this).find('.mainTemplate');
                var iH = inner.height();
                var iW = inner.width();
                var zoom = 1;
                var hR = pH / iH;
                var wR = pW / iW;
                if (hR > wR) {
                    zoom = wR;
                } else {
                    zoom = hR;
                }

                inner.css('zoom', zoom);
            });
        }

        function convertImageToBase64(element, isText,isEdit) {
            var MAX_WIDTH = 320;
            var MAX_HEIGHT = 180;
            var MIME_TYPE = "image/jpeg";
            var QUALITY = 0.7;

            var file = element[0].files[0];
            var blobURL = URL.createObjectURL(file);
            var img = new Image();
            img.src = blobURL;
            img.onerror = function () {
                URL.revokeObjectURL(this.src);
                // Handle the failure properly
                console.log("Cannot load image");
            };
            img.onload = function () {
                URL.revokeObjectURL(this.src);
               
                var wh = calculateSize(img, MAX_WIDTH, MAX_HEIGHT);
                newWidth = wh.width;
                newHeight = wh.height;
                var canvas = document.createElement("canvas");
                canvas.width = newWidth;
                canvas.height = newHeight;
                var ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0, newWidth, newHeight);
                canvas.toBlob(function (blob) {
                },
                    MIME_TYPE,
                    QUALITY);
                //console.log(canvas.toDataURL());  
                if (!isEdit) {
                    var itm = '<div class="fieldGroup">'
                        + '<span class="fieldLbl" title="Label" contenteditable="true">Label:</span>';

                    if (isText) {
                        itm += '<img style="width:30px; height:30px" src="' + canvas.toDataURL() + '">';
                        itm += '<span class="fieldValue" contenteditable="true">Value</span>';
                    } else {
                        itm += '<img style="width:50px; height:50px" src="' + canvas.toDataURL() + '">';
                    }
                    itm += '</div>';
                    var blok = DivViewBlock.find('.vis-active-block');
                    if (blok.hasClass('grdDiv')) {
                        blok.append($(itm));
                    }
                } else {
                    DivViewBlock.find('.vis-active-block').attr('src', canvas.toDataURL());
                }
            };            
            
        };


        function calculateSize(img, maxWidth, maxHeight) {
            var width = img.width;
            var height = img.height;

            // calculate the width and height, constraining the proportions
            if (width > height) {
                if (width > maxWidth) {
                    height = Math.round(height * maxWidth / width);
                    width = maxWidth;
                }
            } else {
                if (height > maxHeight) {
                    width = Math.round(width * maxHeight / height);
                    height = maxHeight;
                }
            }
            return {
                width: width,
                height: height
            }
        }

      

        this.show = function (istext,isEdit) {
            var input = $('<input type="file" name="" maxlength="50" style="height: 45px;padding: 10px" class="" accept="image/*" placeholder=" " data-placeholder="">');
            var lbl = $('<label for="">' + VIS.Msg.getMsg("SelectFromFiles") + '</label>');
            var $root = $('<div class="input-group vis-input-wrap"></div>');
            var control = $('<div class="vis-control-wrap"></div>');
            control.append(input).append(lbl);
            $root.append(control);
           
            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("InsertImage"));
            ch.setWidth('30%');
            //ch.setHeight(h);
            ch.setContent($root);
            ch.onOkClick = function (e) {
                convertImageToBase64(input, istext, isEdit);
            };
            ch.onCancelClick = cancel;
            ch.onClose = cancel;
            ch.show();
            //ch.hideButtons();
        }

        function cancel() {
            ch.close();
            return true;
        };


        loadUI();
    }

   
    VCTMaster.prototype.init = function (windowNo, frame) {
        //Assign to this Varable
        this.frame = frame;
        this.windowNo = windowNo;
        this.frame.getContentGrid().append(this.getRoot());
    };

    VCTMaster.prototype.dispose = function () {
        this.disposeComponent();
    };




    VIS.CardTemplateMaster = VCTMaster;

})(VIS, jQuery);