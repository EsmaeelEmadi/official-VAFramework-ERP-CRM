; (function (VIS, $) {

    function cvd(aPanel) {
        var self = this;
        var gc = aPanel.curGC;
        var mTab = gc.getMTab();
        var cardView = gc.vCardView;
        var AD_Window_ID = mTab.getAD_Window_ID();
        var AD_Tab_ID = mTab.getAD_Tab_ID();
        var tabName = mTab.getName();
        var WindowNo = mTab.getWindowNo();
        var WindowName = aPanel.curGC.aPanel.$parentWindow.getName();
        var AD_CardView_ID = cardView.getAD_CardView_ID();
        var AD_GroupField_ID = cardView.getField_Group_ID();
        var tabField = mTab.getFields();
        var findFields = mTab.getFields().slice();
        var control1 = null;
        var control2 = null;
        var divValue1 = null;
        var cardViewColumns = [];
        var columnFieldArray = [];
        var cardViewColArray = [];
        var totalTabFileds = [];
        var orginalIncludedCols = [];
        var cvTable = null;
        var cardviewCondition = [];
        var cvConditionArray = null;
        var chkDefault = null;
        var chkPublic = null;

        var root, ch;
        var btnCardCustomization = null;
        var btnChangeTemplate = null;
        var btnSaveClose = null;
        var DivCradStep1 = null;
        var DivCradStep2 = null;
        var btnLayoutSetting = null;
        var btnFinesh = null;
        var btnOnlySave = null;
        var count = 1;
        var LstCardViewCondition = null;
        var dbResult = null;
        var cardViewInfo = [];
        var cardsList = null;
        var txtCardName = null;
        var cmbColumn = null;
        var drpOp = null;
        var cmbGroupField = null;
        //var availableFeilds = null;
        //var includedFeilds = null;
        var groupSequenceFeilds = null;
        var cmbOrderClause = null;
        var sortList = null;
        var isAsc = "ASC";
        var btnCopy = null;
        var btnEdit = null;
        var btnDelete = null;
        var btnCancle = null;
        var btnNewCards = null;
        var chkGradient = null;
        var txtBgColor = null;
        var DivGrdntBlock = null;
        var txtGrdntColor1 = null;
        var txtGrdntColor2 = null;
        var txtGrdntPrcnt = null;
        var btnAddCondition = null;
        var isEdit = false;
        var isNewRecord = false;
        var closeDialog = true;
        var btnAddOrder = null;
        var isBusyRoot = null;
        var isSameUser = true;
        var lastSelectedID = null;
        var mdown = false;
        var DivViewBlock = null;
        var DivStyleSec1 = null;
        var chkAllBorderRadius = null;
        var chkAllPadding = null;
        var chkAllMargin = null;
        var chkAllBorder = null;
        var DivTemplate = null;
        var DivGridSec = null;
        var startRowIndex = null;
        var startCellIndex = null;
        var txtRowGap = null;
        var txtColGap = null;
        var DivCardField = null;
        var activeSection = null;
        //var txtTemplateName = null;
        var sectionCount = 2;
        var AD_HeaderLayout_ID = 0;
        var templateID = 0;
        var templateName = null;
        var txtCustomStyle = null;
        var txtSQLQuery = null;
        var CardCreatedby = null;
        var hideShowGridSec = null;
        var txtZoomInOut = null;
        var divTopNavigator = null;
        var btn_BlockCancel = null;
        var btnTemplateBack = null;
        var spnLastSaved = null;
        var isChange = false;
        var isOnlySave = false;
        var isSystemTemplate = 'Y';
        var isCopy = false;
        var dragged = null;
        var gridObj = {
        };
        function init() {
            root = $('<div style="height:100%"></div>');
            isBusyRoot = $("<div class='vis-apanel-busy vis-cardviewmainbusy'></div> ");           
            CardViewUI();
        }

        function IsBusy(isBusy) {
            if (isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "block" });
            }
            if (!isBusy && isBusyRoot != null) {
                isBusyRoot.css({ "display": "none" });
            }
        };

        // #region Step 1 Events

        function events() {
            /*Step 1 Events*/
            btnCardCustomization.click(function (e) {
                closeDialog = false; 
                divTopNavigator.hide();
                count=1;
                DivTemplate.show();
                DivCardFieldSec.hide();               
                DivCradStep1.hide();
                DivCradStep2.show();              
                DivTemplate.find('.mainTemplate[templateid="' + AD_HeaderLayout_ID + '"]').parent().click();
                DivStyleSec1.hide();
                DivCradStep2.find('.vis-two-sec-two').hide();
                if (!isNewRecord) {
                    btnLayoutSetting.click();
                }
            });

            btnSaveClose.click(function (e) {
                closeDialog = true;
                cardViewColArray = [];
                SaveChanges(e);
            });

            btnTemplateBack.click(function (e) {
                if (AD_CardView_ID == "undefined") {
                    btnBack.click();
                } else {
                    count++;
                    DivTemplate.hide();
                    DivCardFieldSec.show();
                    DivCradStep2.find('.vis-two-sec-two').show();
                    DivStyleSec1.show();
                }
            });

            btnNewCard.click(function () {
                isEdit = false;
                isNewRecord = true;
                resetAll();
                enableDisable(true);
                lastSelectedID = cardsList.find('.crd-active').attr('idx');
                cardsList.find('.crd-active').removeClass('crd-active');
                template = '<div class="vis-lft-sgl p-2 d-flex flex-column mb-2 crd-active">';

                template += '<span class="vis-lft-sgl-title">--</span>'
                    + '    <span class="vis-lft-sgl-sub-title">Created By: ' + VIS.context.getAD_User_Name() + '</span>'
                    + '    <span class="vis-lft-sgl-sub-title">Last Modified: ' + new Date().toLocaleDateString() + '</span>'
                    + '</div>';

                cardsList.prepend($(template));
                AD_HeaderLayout_ID = 0;
                AD_CardView_ID = "undefined"; 
                btnTemplateBack.text(VIS.Msg.getMsg("Back"));
                btnLayoutSetting.text(VIS.Msg.getMsg("NextLayout"));
                btnCardCustomization.click();
            });

            //btnEdit.click(function () {
            //    isEdit = true;
            //    isNewRecord = false;
            //    enableDisable(true);
            //    chkDefault.parent().hide();
            //});

            btnCopy.click(function () {                
                var newCopyCard = new cardCopyDialog();
                newCopyCard.show();
                newCopyCard.onSave = function () {
                    var newcardname = newCopyCard.getName();
                    if (newcardname && newcardname.length > 0) {
                        IsBusy(true);
                        saveCopyCard(newcardname);
                    }
                }
            });

            btnCancle.click(function () {
                isNewRecord = false;
                isEdit = true;
                enableDisable(false);
                if (lastSelectedID) {
                    cardsList.find('.crd-active').remove();
                    cardsList.find("[idx='" + lastSelectedID + "']").addClass('crd-active');
                    lastSelectedID = null;
                }
                cardsList.find('.crd-active').trigger('click');
            });

            btnLayoutSetting.click(function () {   
                addSelectedTemplate();
                count++;
                fillcardLayoutfromTemplate(); 
                DivTemplate.hide();               
                DivCardFieldSec.show();
                DivCradStep2.find('.vis-two-sec-two').show();
                DivStyleSec1.show();
                if (AD_HeaderLayout_ID == 0 && DivGridSec.find('.rowBox').length==1) {
                    DivGridSec.find('.addGridCol').click();
                    DivGridSec.find('.addGridRow').click();
                }
                if (isCopy) {
                    setTimeout(function () {
                        btnFinesh.click();
                    }, 2000);
                }
                //FillFields(true, false);
            });

            btnChangeTemplate.click(function () {
                btnTemplateBack.text(VIS.Msg.getMsg("Cancle"));
                btnLayoutSetting.text(VIS.Msg.getMsg("Ok"));
                divTopNavigator.hide();
                count--;
                DivTemplate.show();                
                DivCardFieldSec.hide();
                DivStyleSec1.hide();
                DivCradStep2.find('.vis-two-sec-two').hide();
            });
            

            btnFinesh.click(function (e) {    
                isOnlySave = false;
                saveTemplate(e);
            });

            btnOnlySave.click(function (e) {
                isOnlySave = true;
                saveTemplate(e);
                var tme = new Date();
                var dateString = '';
                var h = tme.getHours();
                var m = tme.getMinutes();
                var s = tme.getSeconds();
                var ampm = h >= 12 ? 'PM' : 'AM';
                h %= 12;
                h = h || 12;  
                if (h < 10) h = '0' + h;
                if (m < 10) m = '0' + m;
                if (s < 10) s = '0' + s;

                dateString = h + ':' + m + ':' + s + ' ' + ampm;

                spnLastSaved.text(VIS.Msg.getMsg("LastSaved") + " " + dateString);
                DivTemplate.find('.mainTemplate[templateid="' + AD_HeaderLayout_ID + '"]').parent().attr('lastupdated', dateString);
            });

            btnBack.click(function () {
                resetAll();
                DivCradStep1.show();
                DivCradStep2.hide();
                btnCancle.click();
            });

            btnDelete.click(function () {
                VIS.ADialog.confirm("SureWantToDelete", true, "", VIS.Msg.getMsg("Confirm"), function (result) {
                    if (result) {
                        DeleteCardView();
                    }
                });
            });

            btnCardAsc.on('click', function () {
                if (cmbOrderClause.val() == -1 || cmbOrderClause.val() == null)
                    return;

                btnCardAsc.css('color', 'rgba(var(--v-c-primary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                isAsc = "ASC";
            });

            btnCardDesc.on('click', function () {

                if (cmbOrderClause.val() == -1 || cmbOrderClause.val() == null)
                    return;

                btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-primary), 1)');
                isAsc = "DESC";
            });

            cardsList.find('div').click(function () {
                cardsList.find('.crd-active').removeClass('crd-active');
                $(this).addClass('crd-active');
                cmbOrderClause.val(-1);
                sortOrderArray = [];
                lastSortOrderArray = [];
                LastCVCondition = [];

                var idx = $(this).attr('idx');
                if (cardViewInfo && cardViewInfo.length != 0) {
                    ControlMgmt(idx);
                    txtCardName.val(cardViewInfo[idx].CardViewName);
                    //txtTemplateName.val(cardViewInfo[idx].CardViewName);
                    AD_CardView_ID = cardViewInfo[idx].CardViewID;
                    cardViewUserID = cardViewInfo[idx].CreatedBy;
                    chkDefault.prop("checked", cardViewInfo[idx].DefaultID ? true : false);
                    chkPublic.prop("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                    AD_HeaderLayout_ID = cardViewInfo[idx].AD_HeaderLayout_ID;
                    //templateID = AD_HeaderLayout_ID;
                    if (cardViewInfo && cardViewInfo[idx].OrderByClause && cardViewInfo[idx].OrderByClause.length) {
                        addOrderByClauseFromDB(cardViewInfo[idx].OrderByClause);
                    }
                    else {
                        clearOrderByClause();
                    }

                    if (AD_CardView_ID > 0) {
                        FillFields(true, false);

                    } else {
                        FillFields(false, false);
                    }
                    FillGroupFields();

                }

            });

            groupSequenceFeilds.find('.grpChk').click(function () {
                if ($(this).hasClass('fa-check-square-o')) {
                    $(this).removeClass('fa-check-square-o').addClass('fa-square-o');
                } else {
                    $(this).removeClass('fa-square-o').addClass('fa-check-square-o');
                }
            });

            chkGradient.click(function () {
                if ($(this).is(':checked')) {
                    DivGrdntBlock.show();
                    txtBgColor.hide();
                } else {
                    DivGrdntBlock.hide();
                    txtBgColor.show();
                }
            });

            txtGrdntColor1.on('input', function () {
                updateGradientColor();
            });

            txtGrdntColor2.on('input', function () {
                updateGradientColor();
            });

            txtGrdntPrcnt.on('input', function () {
                updateGradientColor();
            });

            DivGrdntBlock.find('.vis-circular-slider-circle').mousedown(function (e) {
                mdown = true;
            }).mousemove(function (e) {
                if (mdown) {
                    var $slider = DivGrdntBlock.find('.vis-circular-slider-dot')
                    var deg = getGradientDeg($slider, e);
                    $slider.css({ WebkitTransform: 'rotate(' + deg + 'deg)' });
                    $slider.css({ '-moz-transform': 'rotate(' + deg + 'deg)' });
                    $slider.attr("deg", deg);
                    updateGradientColor();
                }
            });

            $('body').mouseup(function (e) {
                mdown = false;
            });

            btnAddCondition.click(function () {
                if (cmbColumn.find(":selected").val() == -1) {
                    return;
                }
                var condition = {};
                cvConditionArray = {};

                var colorValue = "";
                if (chkGradient.is(':checked')) {
                    colorValue = DivGrdntBlock.css('background');
                } else {
                    if (!Modernizr.inputtypes.color) {
                        colorValue = ctrColor.spectrum('get');
                    }
                    else {
                        colorValue = txtBgColor.val();
                    }
                }

                var index = $.map(cardviewCondition, function (value, i) {
                    if (value.Color == colorValue) {
                        return i;
                    }
                });


                if (index.length <= 0) {
                    cvConditionArray["Color"] = colorValue.toString();
                    cvConditionArray["Condition"] = [];
                    condition["ColHeader"] = cmbColumn.find(":selected").text();
                    condition["ColName"] = cmbColumn.find(":selected").val();
                    condition["Operator"] = drpOp.val();
                    condition["OperatorText"] = drpOp.find(":selected").text();;
                    condition["QueryValue"] = getControlValue(true);
                    condition["QueryText"] = getControlText(true);
                    cvConditionArray["Condition"].push(condition);
                    cardviewCondition.push(cvConditionArray);
                }
                else {
                    condition["ColHeader"] = cmbColumn.find(":selected").text();
                    condition["ColName"] = cmbColumn.find(":selected").val();
                    condition["Operator"] = drpOp.val();
                    condition["OperatorText"] = drpOp.find(":selected").text();;
                    condition["QueryValue"] = getControlValue(true);
                    condition["QueryText"] = getControlText(true);
                    cardviewCondition[index[0]].Condition.push(condition);
                }
                AddRow(cardviewCondition);
                cmbColumn.find("[value='" + -1 + "']").attr("selected", "selected");
                drpOp[0].SelectedIndex = -1;
                SetControlValue(true);
                SetControlText(true);
                divValue1.empty();
            });

            btnAddOrder.on("click", function (e) {
                var selectedVal = cmbOrderClause.val();

                if (selectedVal == -1) {
                    return;
                }

                btnCardAsc.css('color', 'rgba(var(--v-c-on-secondary), 1)');
                btnCardDesc.css('color', 'rgba(var(--v-c-on-secondary), 1)');

                if (sortOrderArray && sortOrderArray.length < 3) {
                    var selectedColtext = cmbOrderClause.find(':selected').text();


                    if (sortOrderArray.indexOf(selectedVal + ' ASC') > -1 || sortOrderArray.indexOf(selectedVal + ' DESC') > -1) {
                        VIS.ADialog.warn("CardSortColAdded");
                        return;
                    }

                    addOrderByClauseItems(selectedColtext, selectedVal, isAsc);
                    sortOrderArray.push(selectedVal + ' ' + isAsc);
                    cmbOrderClause.val(-1);
                }
                else {
                    VIS.ADialog.warn("MaxSortColumn");
                }
            });

            sortList.on("click", function (e) {
                if (isEdit || isNewRecord) {
                    var $target = $(e.target);
                    if ($target.hasClass('fa-close'))
                        $target = $target.parent();

                    if ($target.hasClass('vis-sortListItemClose')) {
                        const itemid = sortOrderArray.indexOf($target.data('text'));
                        sortOrderArray.splice(itemid, 1);
                        $target.closest('.vis-sortListItem').remove();
                    }
                }
            });

            if (cmbColumn != null) {
                cmbColumn.on("change", function (evt) {
                    evt.stopPropagation();
                    var columnName = cmbColumn.val();
                    setControlNullValue(true);
                    if (columnName && columnName != "-1") {
                        var dsOp = null;
                        var dsOpDynamic = null;
                        if (columnName.endsWith("_ID") || columnName.endsWith("_Acct") || columnName.endsWith("_ID_1") || columnName.endsWith("_ID_2") || columnName.endsWith("_ID_3")) {
                            // fill dataset with operators of type ID
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.CVOPERATORS_ID);
                        }
                        else if (columnName.startsWith("Is")) {
                            // fill dataset with operators of type Yes No
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.OPERATORS_YN);
                        }
                        else {
                            // fill dataset with all operators available
                            dsOp = self.getOperatorsQuery(VIS.Query.prototype.CVOPERATORS);
                        }

                        var f = mTab.getField(columnName);
                        drpOp.html(dsOp);
                        drpOp[0].SelectedIndex = 0;
                        // get field
                        var field = getTargetMField(columnName);
                        // set control at value1 position
                        setControl(true, field);
                        // enable the save row button
                        // setEnableButton(btnSave, true);//silverlight comment
                        drpOp.prop("disabled", false);
                    }
                    // enable control at value1 position
                    setValueEnabled(true);
                    // disable control at value2 position
                    // setValue2Enabled(false);
                });
            }

            if (drpOp != null) {
                drpOp.on("change", function () {
                    var selOp = drpOp.val();
                    // set control at value2 position according to the operator selected
                    if (!selOp) {
                        selOp = "";
                    }

                    var columnName = "";
                    var field = "";
                    if (selOp && selOp != "0") {
                        //if user selects between operator
                        if (VIS.Query.prototype.BETWEEN.equals(selOp)) {
                            columnName = cmbColumn.val();
                            // get field
                            field = getTargetMField(columnName);
                            // set control at value2 position
                            setControl(false, field);
                            // enable the control at value2 position
                            // setValue2Enabled(true);
                        }
                        else {
                            //setValue2Enabled(false);
                        }
                    }
                    else {
                        setEnableButton(btnSave, false);//
                        // setValue2Enabled(false);
                        setControlNullValue(true);
                    }
                });
            }

            if (cmbGroupField != null) {
                cmbGroupField.on("change", function () {
                    AD_GroupField_ID = parseInt($(this).find(":selected").val());
                    FillforGroupSeq(AD_GroupField_ID);
                });
            }

            if (cvTable != null) {
                cvTable.on("click", "tr .td_Action i", function () {
                    if (isEdit || isNewRecord) {
                        var rowIndex = $(this).parent().parent().index();
                        var selectRowColor = $(this).parent().parent().children().eq(0).attr("value");
                        var colName = $(this).parent().parent().children().eq(1).attr("value");
                        cvTable.find("tr").eq(rowIndex).remove();
                        var idx = $.map(cardviewCondition, function (value, i) {
                            if (value.Color == selectRowColor) {
                                return i;
                            }
                        });
                        for (i = 0; i < cardviewCondition[idx].Condition.length; i++) {

                            if (colName == cardviewCondition[idx].Condition[i].ColName) {
                                cardviewCondition[idx].Condition.splice(i, 1);
                            }
                            if (cardviewCondition[idx].Condition.length <= 0) {
                                cardviewCondition.splice(idx, 1);
                                break;
                            }
                        }
                    }
                    if (cardviewCondition.length == 0) {
                        AddRow("");
                    }
                });
            }

            txtCustomStyle.change(function () {
                var selectedItem = DivViewBlock.find('.vis-active-block');
                selectedItem.attr("style", $(this).val());
            });

            txtSQLQuery.change(function () {
                var selectedItem = DivViewBlock.find('.vis-active-block');
                if ($(this).val() == null || $(this).val() == "") {
                    selectedItem.find('sql').remove();
                    selectedItem.attr("query", "");
                } else {
                    var qry = VIS.secureEngine.encrypt($(this).val());
                    selectedItem.attr("query", qry);
                    if (selectedItem.find('sql').length == 0) {

                        selectedItem.append('<sql>SQL</sql>');
                    } 
                }
                
            });

            txtZoomInOut.on('input', function () {
                DivViewBlock.find('.canvas').css('zoom', $(this).val());
            })

            DivViewBlock.find('.vis-viewBlock')[0].addEventListener("dragover", function (event){
                // prevent default to allow drop
                event.preventDefault();
            });

            DivViewBlock.find('.vis-viewBlock')[0].addEventListener("drop", function (event) {
                // prevent default action (open as link for some elements)
                event.preventDefault();
                DivViewBlock.find('.vis-active-block').removeClass('vis-active-block');
                $(event.target).addClass('vis-active-block');
                // move dragged element to the selected drop target
                linkField(dragged);
            });

            /* End Step 1*/

        }

        // #endregion

        function CardViewUI() {
            root.load(VIS.Application.contextUrl + 'CardViewWizard/Index/?windowno=' + WindowNo, function (event) {
                /*step 1*/
                DivCradStep1 = root.find('#DivCardStep1_' + WindowNo);
                btnCardCustomization = root.find('#btnCardCustomization_' + WindowNo);
                btnSaveClose = root.find('#btnSaveCloseStep1_' + WindowNo);
                btnLayoutSetting = root.find('#BtnLayoutSetting_' + WindowNo);
                btnChangeTemplate = root.find('#BtnChangeTemplate_' + WindowNo);
                btnTemplateBack = root.find('#BtnTemplateBack_' + WindowNo);

                btnBack = root.find('#BtnBack_' + WindowNo);
                btnFinesh = root.find('#BtnFinesh_' + WindowNo);
                btnOnlySave = root.find('#btnOnlySave_' + WindowNo);
                cardsList = root.find('#DivCardList_' + WindowNo);
                txtCardName = root.find('#txtCardName_' + WindowNo);
                cmbGroupField = root.find('#cmbGroupField_' + WindowNo);
                //availableFeilds = root.find('#AvailableFeilds_' + WindowNo);
                //includedFeilds = root.find('#IncludedFeilds_' + WindowNo);
                groupSequenceFeilds = root.find('#GroupSequenceFeilds_' + WindowNo);
                cvTable = root.find('#conditionTable_' + WindowNo);
                cmbColumn = root.find('#cmbColumn_' + WindowNo);
                drpOp = root.find('#ddlOperator_' + WindowNo);
                divValue1 = root.find('#valcontainer_' + WindowNo);
                cmbOrderClause = root.find('#cmbOrderClause_' + WindowNo);
                sortList = root.find('#sortList_' + WindowNo);
                chkDefault = root.find('#chkDefault_' + WindowNo);
                chkPublic = root.find('#chkPublic_' + WindowNo);
                btnNewCard = root.find('#btnNewCard_' + WindowNo);
                btnCopy = root.find('#btnCopy_' + WindowNo);
                //btnEdit = root.find('#btnEdit_' + WindowNo);
                btnDelete = root.find('#btnDelete_' + WindowNo);
                btnCancle = root.find('#btnCancle_' + WindowNo);
                chkGradient = root.find('#chkGradient_' + WindowNo);
                txtBgColor = root.find('#txtBgColor_' + WindowNo);
                DivGrdntBlock = root.find('#DivGrdntBlock_' + WindowNo);
                txtGrdntColor1 = root.find('#txtGrdntColor1_' + WindowNo);
                txtGrdntColor2 = root.find('#txtGrdntColor2_' + WindowNo);
                txtGrdntPrcnt = root.find('#txtGrdntPrcnt_' + WindowNo);
                btnAddCondition = root.find('#btnAddCondition_' + WindowNo);
                btnCardAsc = root.find('#btnCardAsc_' + WindowNo);
                btnCardDesc = root.find('#btnCardDesc_' + WindowNo);
                btnAddOrder = root.find('#btnAddOrder_' + WindowNo);
                spnLastSaved = root.find('#spnLastSaved_' + WindowNo);
                /*END Step 1*/

                /* Step 2*/
                DivCradStep2 = root.find('#DivCardStep2_' + WindowNo);
                DivViewBlock = root.find('#DivViewBlock_' + WindowNo);
                DivStyleSec1 = root.find('#DivStyleSec1_' + WindowNo);
                chkAllBorderRadius = root.find('#chkAllBorderRadius_' + WindowNo);
                chkAllPadding = root.find('#chkAllPadding_' + WindowNo);
                chkAllMargin = root.find('#chkAllMargin_' + WindowNo);
                chkAllBorder = root.find('#chkAllBorder_' + WindowNo);
                DivGridSec = root.find('#DivGridSec_' + WindowNo);
                //hideShowGridSec = root.find('.DivGridSec');
                DivTemplate = root.find('#DivTemplate_' + WindowNo);
                DivCardField = root.find('#DivCardField_' + WindowNo);
                //txtTemplateName = root.find('#txtTemplateName_' + WindowNo);
                DivCardFieldSec = root.find('#DivCardFieldSec_' + WindowNo);
                txtCustomStyle = root.find('#txtCustomStyle_' + WindowNo);
                txtSQLQuery = root.find('#txtSQLQuery_' + WindowNo);
                txtZoomInOut = root.find('#txtZoomInOut_' + WindowNo);
                btn_BlockCancel = root.find('#Btn_BlockCancel_' + WindowNo);
               
                divTopNavigator = DivCradStep2.find('.vis-topNavigator');
                txtRowGap = DivGridSec.find('.rowGap');
                txtColGap = DivGridSec.find('.colGap');
                activeSection = DivViewBlock.find('.section1');


                /*END Step 2*/

                ArrayTotalTabFields();
                GetCards();
                FillFields(true, false);
                FillGroupFields();
                FillCVConditionCmbColumn();
                events();
                events2();
                updateGradientColor();
                getTemplateDesign();
                totalTabFileds.sort(function (a, b) {
                    var n1 = a.getHeader().toUpperCase();
                    if (n1 == null || n1.length == 0) {
                        n1 = VIS.Msg.getElement(VIS.context, a.getColumnName());
                    }
                    var n2 = b.getHeader().toUpperCase();
                    if (n2 == null || n2.length == 0) {
                        n2 = VIS.Msg.getElement(VIS.context, b.getColumnName());
                    }
                    if (n1 > n2) return 1;
                    if (n1 < n2) return -1;
                    return 0;
                });
                enableDisable(false);
                cmbOrderClause.find('option').remove();
                cmbOrderClause.append('<option value="-1"></option>)');

                for (var j = 0; j < totalTabFileds.length; j++) {
                    var header = totalTabFileds[j].getHeader();
                    if (header == null || header.length == 0) {
                        header = VIS.Msg.getElement(VIS.context, totalTabFileds[j].getColumnName());
                        if (header == null || header.Length == 0)
                            continue;
                    }

                    cmbOrderClause.append('<option value="' + totalTabFileds[j].getColumnName() + '">' + header + '</option>')
                }

                if (cardViewInfo && cardViewInfo.length == 0) {
                    isSameUser = true;
                    btnNewCard.click();
                    btnCancle.addClass('vis-disable-event');
                }

                isEdit = true;
                isNewRecord = false;
                chkDefault.parent().hide();

                cardsList.scrollTop(cardsList.find('.crd-active').offset().top - cardsList.find('.crd-active').height() - 100);
                root.append(isBusyRoot);
            });


        }

        // #region Step 1 functions
        var updateGradientColor = function () {
            var color1 = txtGrdntColor1.val();
            var color2 = txtGrdntColor2.val();
            var prct = txtGrdntPrcnt.val();
            var deg = DivGrdntBlock.find('.vis-circular-slider-dot').attr('deg');
            var style = 'linear-gradient(' + deg + 'deg,' + color1 + ' ' + prct + '%,  ' + color2 + ')';
            DivGrdntBlock.find('.vis-left-grad-side').css('background-color', color1);
            DivGrdntBlock.find('.vis-right-grad-side').css('background-color', color2);
            DivGrdntBlock.css('background', style);
        }

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

        var ArrayTotalTabFields = function () {
            for (var i = 0; i < mTab.getFields().length; i++) {
                totalTabFileds.push(mTab.getFields()[i]);
            }

            for (var i = 0; i < cardView.fields.length; i++) {
                orginalIncludedCols.push(cardView.fields[i].getAD_Field_ID());
            }
        };

        var GetCards = function (isDelete) {
            var url = VIS.Application.contextUrl + "CardView/GetCardView";
            $.ajax({
                type: "GET",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: { ad_Window_ID: AD_Window_ID, ad_Tab_ID: AD_Tab_ID },
                success: function (data) {
                    dbResult = JSON.parse(data);
                    cardViewInfo = dbResult[0].lstCardViewData;
                    //roleInfo = dbResult[0].lstRoleData;
                    //LstCardViewRole = dbResult[0].lstCardViewRoleData;
                    LstCardViewCondition = dbResult[0].lstCardViewConditonData;

                    if (cardViewInfo != null && cardViewInfo.length > 0) {
                        var isDefaultcard = false;
                        for (var i = 0; i < cardViewInfo.length; i++) {
                            var template = "";
                            if (cardViewInfo[i].DefaultID) {
                                isDefaultcard = true;
                                template = '<div idx="' + i + '" class="vis-lft-sgl p-2 d-flex flex-column mb-2 crd-active">';
                            } else {
                                template = '<div idx="' + i + '" class="vis-lft-sgl p-2 d-flex flex-column mb-2">';
                            }

                            template += '<span class="vis-lft-sgl-title">' + w2utils.encodeTags(cardViewInfo[i].CardViewName) + '</span>'
                                + '    <span class="vis-lft-sgl-sub-title">Created By: ' + cardViewInfo[i].CreatedName + '</span>'
                                + '    <span class="vis-lft-sgl-sub-title">Last Modified: ' + new Date(cardViewInfo[i].Updated).toLocaleDateString() + '</span>'
                                + '</div>';

                            cardsList.append($(template));

                            //cardsList.append("<Option idx=" + i + " is_shared=" + cardViewInfo[i].UserID + " ad_user_id=" + cardViewInfo[i].CreatedBy + " cardviewid=" + cardViewInfo[i].CardViewID + " groupSequence='" + cardViewInfo[i].groupSequence + "' excludedGroup='" + cardViewInfo[i].excludedGroup + "'  ad_field_id=" + cardViewInfo[i].AD_GroupField_ID + " isdefault=" + cardViewInfo[i].DefaultID + " ad_headerLayout_id=" + cardViewInfo[i].AD_HeaderLayout_ID + "> " + w2utils.encodeTags(cardViewInfo[i].CardViewName) + "</Option>");
                        }
                        if (!isDefaultcard) {
                            cardsList.find('div:first').addClass("crd-active");
                        }
                        var idx = cardsList.find(".crd-active").attr('idx');
                        AD_CardView_ID = cardViewInfo[idx].CardViewID;
                        txtCardName.val(cardViewInfo[idx].CardViewName);
                        //txtTemplateName.val(cardViewInfo[idx].CardViewName);
                        AD_HeaderLayout_ID = cardViewInfo[idx].AD_HeaderLayout_ID;
                        ControlMgmt(idx);
                        chkPublic.attr("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                        chkDefault.attr("checked", cardViewInfo[idx].DefaultID ? true : false);

                        if (idx && cardViewInfo[idx].OrderByClause && cardViewInfo[idx].OrderByClause.length > 0) {
                            addOrderByClauseFromDB(cardViewInfo[idx].OrderByClause);
                        }
                        else {
                            clearOrderByClause();
                        }
                    } else {
                        cardViewInfo = [];
                    }

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                }
            });


        };

        var fillcardLayoutfromTemplate = function(){
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

            DivCardField.find('.fieldLbl[seqNo]').each(function (i) {
                var fID = $(this).attr('fieldid');
                if (DivViewBlock.find('[fieldid="' + fID + '"]').length == 0) {
               
                var vlu = $(this).text();
                    var fidItm = DivViewBlock.find('[seqNo="' + $(this).attr('seqNo') + '"]');
                    fidItm.html('');
                    if (fidItm.length == 0) {
                        $(this).find('.linked').removeClass('linked vis-succes-clr');
                    } else {

                        var vlstyle = "";
                        var imgStyle = "";
                        var spnStyle = "";
                        var styleArr = fidItm.attr("fieldValuestyle");
                        if (styleArr && styleArr.indexOf('|') > -1) {
                            styleArr = styleArr.split("|");
                            if (styleArr && styleArr.length > 0) {
                                for (var j = 0; j < styleArr.length; j++) {
                                    if (styleArr[j].indexOf("@img::") > -1) {
                                        imgStyle = styleArr[j].replace("@img::", "");
                                    }
                                    else if (styleArr[j].indexOf("@value::") > -1) {
                                        vlstyle = styleArr[j].replace("@value::", "");
                                    } else if (styleArr[j].indexOf("@span::") > -1) {
                                        spnStyle = styleArr[j].replace("@span::", "");
                                    }
                                }
                            }
                        } else {
                            vlstyle = styleArr;
                        }
                    }

                    if (fidItm.length > 0) {
                        var fieldHtml = $('<div class="fieldGroup">'
                            + '</div>');
                        var hideIcon = fidItm.attr("showfieldicon") == 'Y' ? true : false;
                        var hideTxt = fidItm.attr("showfieldtext") == 'Y' ? true : false;
                        if (mTab.getFieldById(Number(fID)).getShowIcon()) {
                            if (hideIcon) {
                                fieldHtml.append($('<i class="">&nbsp;</i>'));
                            } else {
                                fieldHtml.append($('<i class="fa fa-star">&nbsp;</i>'));
                            }
                        }
                        var cls = hideTxt ? "displayNone" : "";                       
                        var src = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='50' height='50'%3E%3Cdefs%3E%3Cpath d='M23 31l-3.97-2.9L19 28l-.24-.09.19.13L13 33v2h24v-2l-3-9-5-3-6 10zm-2-12c0-1.66-1.34-3-3-3s-3 1.34-3 3 1.34 3 3 3 3-1.34 3-3zm-11-8c-.55 0-1 .45-1 1v26c0 .55.45 1 1 1h30c.55 0 1-.45 1-1V12c0-.55-.45-1-1-1H10zm28 26H12c-.55 0-1-.45-1-1V14c0-.55.45-1 1-1h26c.55 0 1 .45 1 1v22c-.3.67-.63 1-1 1z' id='a'/%3E%3C/defs%3E%3Cuse xlink:href='%23a' fill='%23fff'/%3E%3Cuse xlink:href='%23a' fill-opacity='0' stroke='%23000' stroke-opacity='0'/%3E%3C/svg%3E";
                        var displayType = mTab.getFieldById(Number(fID)).getDisplayType();
                        if (displayType == VIS.DisplayType.Image) {
                            fieldHtml.append($('<span class="fieldLbl ' + cls + '" draggable="false" showFieldText="' + hideTxt + '" showFieldIcon="' + hideIcon + '" ondragstart="drag(event)" title="' + vlu + '" fieldid="' + fID + '" id="' + $(this).attr('id') + '">' + vlu + '</span><img class="vis-colorInvert" style="' + imgStyle + '" src="' + src + '"/>'));
                        } else if (displayType == VIS.DisplayType.TableDir || displayType == VIS.DisplayType.Table || displayType == VIS.DisplayType.List) {
                            var iconClass = hideIcon ? "displayNone" : ""; 
                            fieldHtml.append($('<span class="fieldLbl ' + cls + '" draggable="false" showFieldText="' + hideTxt + '" showFieldIcon="' + hideIcon + '" ondragstart="drag(event)" title="' + vlu + '" fieldid="' + fID + '" id="' + $(this).attr('id') + '">' + vlu + '</span><img class="' + iconClass+' vis-colorInvert" style="' + imgStyle + '" src="' + src + '"/><span class="fieldValue" style="' + vlstyle + '">Value</span>'));
                        }
                        else {

                            fieldHtml.append($('<span class="fieldLbl ' + cls +'" draggable="false" showFieldText="' + hideTxt + '" showFieldIcon="' + hideIcon + '" ondragstart="drag(event)" title="' + vlu + '" fieldid="' + fID + '" id="' + $(this).attr('id') + '">' + vlu + '</span><span class="fieldValue" style="' + vlstyle + '">:Value</span>'));
                        }

                        if (fidItm.attr("query") != null && fidItm.attr("query") != "") {
                            fieldHtml.append('<sql>SQL</sql>');
                        }
                        fidItm.append(fieldHtml);
                        //$(this).remove();
                    }
                }
            });
        }

        var FillFields = function (isReload, isShowAllColumn) {
            //if (!isReload) {
            //var feildClone = availableFeilds.find('.vis-sec-2-sub-itm:first').clone(true);
            //availableFeilds.find('.vis-sec-2-sub-itm').remove();

            fields = null;
            dbResult = null;

            tabField = mTab.getFields();
            tabField.sort(function (a, b) {
                var a1 = a.getHeader().toLower(), b1 = b.getHeader().toLower();
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });
            FillIncluded(isReload);
            if (mTab != null && mTab.getFields().length > 0) {

                var iClone = DivCardField.find('.fieldLbl:first').clone(true);
                iClone.removeClass('displayNone');

                for (var i = 0; i < tabField.length; i++) {
                    var c = tabField[i].getColumnName().toLower();
                    if (c == "created" || c == "createdby" || c == "updated" || c == "updatedby") {
                        continue;
                    }

                    if (VIS.DisplayType.IsLOB(tabField[i].getDisplayType())) {
                        continue;
                    }

                    if (!tabField[i].getIsDisplayed()) {
                        continue;
                    }

                    if (cardView.hasIncludedCols && !isShowAllColumn) {
                        var result = jQuery.grep(columnFieldArray, function (value) {
                            return value == tabField[i].getAD_Field_ID();
                        });
                        if (result.length > 0) {

                            continue;
                        }
                    }

                    //feildClone.find('.vis-sub-itm-title').text(tabField[i].getHeader());
                    //feildClone.attr("fieldid", tabField[i].getAD_Field_ID());
                    //availableFeilds.append(feildClone);
                    //feildClone = availableFeilds.find('.vis-sec-2-sub-itm:first').clone(true);

                    iClone.prepend(tabField[i].getHeader()).attr("title", tabField[i].getHeader());
                    if (tabField[i].getShowIcon()) {
                        iClone.attr("showfieldicon", false);
                    }

                    if (tabField[i].getDisplayType() == VIS.DisplayType.Image) {
                        iClone.attr("fieldid", tabField[i].getAD_Field_ID()).attr("displayType", "img");
                    } else {
                    iClone.attr("fieldid", tabField[i].getAD_Field_ID());
                    }
                    iClone.attr("id", WindowNo + "_" + tabField[i].getAD_Field_ID());
                    DivCardField.append(iClone);
                    iClone = DivCardField.find('.fieldLbl:first').clone(true);
                    iClone.removeClass('displayNone');

                    //availableFeilds.append("<li index=" + i + " FieldID=" + tabField[i].getAD_Field_ID() + "> <span>" + tabField[i].getHeader() + "</span></li>");
                }
            }

            //availableFeilds.sortable({
            //    connectWith: ".connectedSortable",
            //    disabled: true
            //}).disableSelection();

            //includedFeilds.sortable({
            //    connectWith: ".connectedSortable",
            //    disabled: true
            //}).disableSelection();
        };

        var FillIncluded = function (isReload) {
            //var IncldfeildClone = includedFeilds.find('.displayNone').clone(true);
            //IncldfeildClone.removeClass('displayNone');
            //includedFeilds.find('.vis-sec-2-sub-itm:not(.displayNone)').remove();  
            
            var iClone = DivCardField.find('.fieldLbl:first').clone(true);
            DivCardField.find('.fieldLbl:not(:first)').remove();
            iClone.removeClass('displayNone');

            cardViewColArray = [];
            cardViewColumns = [];
            columnFieldArray = [];
            if (isReload && (AD_CardView_ID > 0 || typeof (AD_CardView_ID) == "undefined")) {
                if (typeof (AD_CardView_ID) == "undefined") {
                    AD_CardView_ID = 0;
                }
                var url = VIS.Application.contextUrl + "CardView/GetCardViewColumns";
                $.ajax({
                    type: "GET",
                    async: false,
                    url: url,
                    dataType: "json",
                    contentType: 'application/json; charset=utf-8',
                    data: { ad_CardView_ID: AD_CardView_ID },
                    success: function (data) {
                        dbResult = JSON.parse(data);
                        var CVColumns = dbResult[0].lstCardViewData;
                        LstCardViewCondition = dbResult[0].lstCardViewConditonData;
                        if (CVColumns != null && CVColumns.length > 0) {
                            AD_GroupField_ID = CVColumns[0].AD_GroupField_ID;
                            cardViewUserID = CVColumns[0].CreatedBy;
                            for (var i = 0; i < CVColumns.length; i++) {
                                if (CVColumns[i].AD_Field_ID == 0) {
                                    continue;
                                }
                                var fieldItem = jQuery.grep(totalTabFileds, function (value) {
                                    return value.getAD_Field_ID() == CVColumns[i].AD_Field_ID
                                });
                                if (fieldItem.length > 0) {
                                    columnFieldArray.push(fieldItem[0].getAD_Field_ID());
                                }
                                //IncldfeildClone.find('.vis-sub-itm-title').text(CVColumns[i].FieldName);
                                //IncldfeildClone.attr("fieldid", CVColumns[i].AD_Field_ID);
                                //includedFeilds.append(IncldfeildClone);
                                //IncldfeildClone = includedFeilds.find('.vis-sec-2-sub-itm:last').clone(true);

                                iClone.prepend(CVColumns[i].FieldName).attr("title", CVColumns[i].FieldName);

                                //iClone.find('.vis-change-clr-red').addClass('linked').removeClass('vis-change-clr-red');
                                //iClone.find('.fa-chain-broken').addClass('fa-link vis-succes-clr').removeClass('fa-chain-broken');
                                if (DivViewBlock.find('.canvas [seqNo="' + CVColumns[i].SeqNo + '"]').length>0) {
                                    iClone.find('.fa-circle').addClass('linked vis-succes-clr');
                                    iClone.prop("draggable", false);
                                } else {
                                    iClone.prop("draggable", true);
                                }

                                if (mTab.getFieldById(CVColumns[i].AD_Field_ID).getShowIcon()) {
                                    iClone.attr("showfieldicon", false);
                                }
                                
                                if (mTab.getFieldById(CVColumns[i].AD_Field_ID).getDisplayType() == VIS.DisplayType.Image) {
                                    iClone.attr("fieldid", CVColumns[i].AD_Field_ID).attr("seqNo", CVColumns[i].SeqNo).attr("displayType","img");
                                } else {
                                    iClone.attr("fieldid", CVColumns[i].AD_Field_ID).attr("seqNo", CVColumns[i].SeqNo);
                                }

                                
                                iClone.attr("id", WindowNo + "_" + CVColumns[i].AD_Field_ID);
                                DivCardField.append(iClone);
                                iClone = DivCardField.find('.fieldLbl:first').clone(true);
                                iClone.removeClass('displayNone');



                                // ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + CVColumns[i].AD_Field_ID + "> <span>" + CVColumns[i].FieldName + "</span></li>");

                            }

                            DivCardField[0].addEventListener("dragstart", function (event){
                                // store a ref. on the dragged elem
                                dragged = $(event.target);
                            });
                        }
                        if (LstCardViewCondition != null && LstCardViewCondition.length > 0) {
                            cardviewCondition = [];
                            FillCVConditonTable(LstCardViewCondition);
                        }
                        else {
                            cardviewCondition = [];
                            AddRow(cardviewCondition);
                        }

                        addOrderByClauseFromDB(CVColumns[0].OrderByClause);
                    }, error: function (errorThrown) {
                        alert(errorThrown.statusText);
                    }
                });
            }
            else if (cardView.hasIncludedCols) {
                var fieldItem = null;
                columnFieldArray = [];
                var includedFields = cardView.fields;
                //cardViewColumns = cardView.fields;
                if (includedFields != null && includedFields.length > 0) {
                    for (var i = 0; i < includedFields.length; i++) {
                        fieldItem = jQuery.grep(totalTabFileds, function (value) {
                            return value.getAD_Field_ID() == includedFields[i].getAD_Field_ID()
                        });
                        if (fieldItem.length > 0) {
                            columnFieldArray.push(fieldItem[0].getAD_Field_ID());
                        }

                        cardViewColArray.push({ AD_Field_ID: includedFields[i].getAD_Field_ID(), CardViewID: AD_CardView_ID, SeqNo: 0, FieldName: includedFields[i].getHeader() });
                               
                        
                        iClone.prepend(includedFields[i].getHeader()).attr("title", includedFields[i].getHeader());
                        if (DivViewBlock.find('[seqNo="' + includedFields[i].SeqNo + '"]')) {
                            iClone.find('.fa-circle').addClass('linked vis-succes-clr');
                        }
                       

                        if (mTab.getFieldById(includedFields[i].getAD_Field_ID()).getShowIcon()) {
                            iClone.attr("showfieldicon", false);
                        }

                        if (mTab.getFieldById(includedFields[i].getAD_Field_ID()).getDisplayType() == VIS.DisplayType.Image) {
                            iClone.attr("fieldid", includedFields[i].getAD_Field_ID()).attr("seqNo", includedFields[i].SeqNo).attr("displayType", "img");
                        } else {
                            iClone.attr("fieldid", includedFields[i].getAD_Field_ID()).attr("seqNo", includedFields[i].SeqNo);
                        }

                        iClone.attr("fieldid", includedFields[i].getAD_Field_ID()).attr("seqNo", includedFields[i].SeqNo);
                        iClone.attr("id", WindowNo + "_" + includedFields[i].getAD_Field_ID());
                        DivCardField.append(iClone);
                        iClone = DivCardField.find('.fieldLbl:first').clone(true);
                        iClone.removeClass('displayNone');
                    }
                }
            }
        }

        var FillGroupFields = function () {
            if (cmbGroupField != null) {
                cmbGroupField.children().remove();
            }
            var fields = null;
            var dbResult = null;
            lovcardList = {};
            if (mTab != null && mTab.getFields().length > 0) {
                cmbGroupField.append("<Option value=" + -1 + "></Option>");
                tabField = mTab.getFields();
                for (var i = 0; i < tabField.length; i++) {
                    var c = tabField[i].getColumnName().toLower();
                    if (c == "created" || c == "createdby" || c == "updated" || c == "updatedby") {
                        continue;
                    }

                    if ((VIS.DisplayType.IsLookup(tabField[i].getDisplayType()) && tabField[i].getLookup() && tabField[i].getLookup().getIsValidated() && tabField[i].getIsDisplayed()) || tabField[i].getDisplayType() == VIS.DisplayType.YesNo) {
                        cmbGroupField.append("<Option value=" + tabField[i].getAD_Field_ID() + "> " + tabField[i].getHeader() + "</Option>");
                        if (tabField[i].getDisplayType() == VIS.DisplayType.List || tabField[i].getDisplayType() == VIS.DisplayType.TableDir || tabField[i].getDisplayType() == VIS.DisplayType.Table || tabField[i].getDisplayType() == VIS.DisplayType.Search) {
                            if (tabField[i].lookup && tabField[i].lookup.getData()) {
                                lovcardList[tabField[i].getAD_Field_ID()] = tabField[i].lookup.getData();
                            }
                        }
                    }
                }

            }
            if (AD_GroupField_ID != null && AD_GroupField_ID > 0) {
                var result = jQuery.grep(tabField, function (value) {
                    return value.getAD_Field_ID() == AD_GroupField_ID;
                });
                cmbGroupField.find("[value='" + AD_GroupField_ID + "']").attr("selected", "selected");
            }
            FillforGroupSeq(AD_GroupField_ID);
        };

        var FillforGroupSeq = function (fieldID) {
            groupSequenceFeilds.find('.onlyLOV').remove();
            var GrpSeqfeildClone = groupSequenceFeilds.find('.vis-sec-2-sub-itm:hidden').clone(true);
            GrpSeqfeildClone.removeClass("displayNone");
            groupSequenceFeilds.find('.vis-sec-2-sub-itm:not(:hidden)').remove();
            if (lovcardList[fieldID]) {
                for (var i = 0, ln = lovcardList[fieldID]; i < ln.length; i++) {
                    if (ln[i].Key.toString().length > 0 && ln[i].Name.toString().length > 0) {
                        GrpSeqfeildClone.attr("key", ln[i].Key);
                        GrpSeqfeildClone.find('.vis-sub-itm-title').text(ln[i].Name);
                        groupSequenceFeilds.append(GrpSeqfeildClone);
                        GrpSeqfeildClone = groupSequenceFeilds.find('.vis-sec-2-sub-itm:last').clone(true);
                        //ulGroupSeqColumns.append('<li key="' + ln[i].Key + '"><input type="checkbox"/>' + ln[i].Name + '</li>');
                    }
                };
                //ulGroupSeqColumns.find('input').prop('checked', true)
                var idx = cardsList.find(".crd-active").attr('idx');
                var seq = cardViewInfo[idx].groupSequence;
                var excGrp = cardViewInfo[idx].excludedGroup;
                if (seq) {
                    seq = seq.split(",");
                    excGrp = excGrp.split(",");
                    for (var j = 0; j < seq.length; j++) {
                        var item = groupSequenceFeilds.find("[key='" + seq[j] + "']");
                        if (excGrp.lastIndexOf(seq[j]) != -1) {
                            item.find('.fa-check-square-o').removeClass('fa-check-square-o').addClass('fa-square-o');
                            //item.find('input').prop('checked', false);
                        }

                        var before = groupSequenceFeilds.find(".vis-sec-2-sub-itm").eq(j);
                        item.insertBefore(before);
                    }
                }
                groupSequenceFeilds.closest('.vis-sec-2-wrapper').css('height', '100%');
                var h = $(window).height() - 248;
                groupSequenceFeilds.css('height', h + 'px');
                groupSequenceFeilds.sortable({
                    disabled: false
                });
            } else {
                //ulGroupSeqColumns.parent().css("background-color", "rgba(var(--v-c-on-secondary), 0.04)");
                groupSequenceFeilds.closest('.vis-sec-2-wrapper').css('height', '100%');
                groupSequenceFeilds.append('<div class="onlyLOV"  key=""><span>' + VIS.Msg.getMsg("OnlyForLOV") + '</span></div>');
                groupSequenceFeilds.sortable({
                    disabled: true
                });
            }
            
        }

        var FillCVConditionCmbColumn = function () {
            var html = '<option value="-1"> </option>';
            for (var c = 0; c < findFields.length; c++) {
                // get field
                var field = findFields[c];

                if (field.getDisplayType() == VIS.DisplayType.Image) {
                    continue;
                }

                if (field.getIsEncrypted())
                    continue;
                // get field's column name
                var columnName = field.getColumnName();
                if (field.getDisplayType() == VIS.DisplayType.Button) {
                    if (field.getAD_Reference_Value_ID() == 0)
                        continue;
                    if (columnName.endsWith("_ID"))
                        field.setDisplayType(VIS.DisplayType.Table);
                    else
                        field.setDisplayType(VIS.DisplayType.List);
                    //field.loadLookUp();
                }

               

                // get text to be displayed
                var header = field.getHeader();
                if (header == null || header.length == 0) {
                    // get text according to the language selected
                    header = VIS.Msg.getElement(VIS.context, columnName);
                    if (header == null || header.Length == 0)
                        continue;
                }
                // if given field is any key, then add "(ID)" to it
                if (field.getIsKey())
                    header += (" (ID)");

                // add a new row in datatable and set values
                //dr = dt.NewRow();
                //dr[0] = header; // Name
                //dr[1] = columnName; // DB_ColName
                //dt.Rows.Add(dr);
                html += '<option value="' + columnName + '">' + header + '</option>';
            }
            cmbColumn.html(html);
        };

        function FillCVConditonTable(data) {
            var condition = {};
            cvConditionArray = {};
            var strConditionValue = "";
            var strConditionText = "";

            var s = null;
            var st = null;
            var colHeader = null;
            var colVaue = null;
            var queryValue = null;
            var queryText = null;
            var operator = null;
            var operatorText = null;
            for (var i = 0; i < data.length; i++) {
                cvConditionArray = {};
                cvConditionArray["Color"] = data[i].Color;
                cvConditionArray["Condition"] = [];
                strConditionValue = data[i].ConditionValue.split("&");
                strConditionText = data[i].ConditionText.split("&");

                for (var j = 0; j < strConditionValue.length; j++) {
                    condition = {}
                    if (strConditionValue[j].contains("!")) {
                        s = strConditionValue[j].trim().split("!");
                        st = strConditionText[j].trim().split("!");
                        operator = "!";
                        operatorText = "!=";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains("=")) {
                        s = strConditionValue[j].split("=");
                        st = strConditionText[j].split("=");
                        operator = "="
                        operatorText = "=";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains("<")) {
                        s = strConditionValue[j].split("<");
                        st = strConditionText[j].split("<");
                        operator = "<";
                        operatorText = "<";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }
                    else if (strConditionValue[j].contains(">")) {
                        s = strConditionValue[j].split(">");
                        st = strConditionText[j].split(">");
                        operator = ">";
                        operatorText = ">";
                        colHeader = st[0].trim().substring(0 + 1, st[0].lastIndexOf("@"));
                        colVaue = s[0].trim().substring(0 + 1, s[0].lastIndexOf("@"));
                        queryText = st[1];
                        queryValue = s[1];
                    }

                    condition["ColHeader"] = colHeader;
                    condition["ColName"] = colVaue;
                    condition["Operator"] = operator;
                    condition["OperatorText"] = operatorText;
                    condition["QueryValue"] = queryValue;
                    condition["QueryText"] = queryText;
                    cvConditionArray["Condition"].push(condition);

                }
                cardviewCondition.push(cvConditionArray);
            }
            //if (!isFirstLoad) {
            AddRow(cardviewCondition);
            //}
        };

        var AddRow = function (data) {
            var rowClone = cvTable.find('tr:first').clone(true);
            rowClone.removeAttr('style');
            cvTable.find('tr:not(:first)').remove();
            if (data.length>0) {
                for (var i = 0; i < data.length; i++) {
                    for (var j = 0; j < data[i].Condition.length; j++) {
                        rowClone.find('.td_bgColor i').css({ 'background': data[i].Color, 'color': 'transparent' });
                        rowClone.find('.td_bgColor').attr("value", data[i].Color);
                        rowClone.find('.td_column').text(data[i].Condition[j].ColHeader).attr('value', data[i].Condition[j].ColName);
                        rowClone.find('.td_operator').text(data[i].Condition[j].Operator).attr('value', data[i].Condition[j].OperatorText);
                        rowClone.find('.td_queryValue').text(data[i].Condition[j].QueryValue).attr('value', data[i].Condition[j].QueryText);
                        cvTable.append(rowClone);
                        rowClone = cvTable.find('tr:last').clone(true);
                    }
                }
            } else {
                cvTable.append("<tr style='height:100%'><td colspan='5' style='background-color: #f1f1f173;'><div class='align-items-center d-flex justify-content-center'><i class='fa fa-database mr-1 fa-2x' aria-hidden='true'></i>" + VIS.Msg.getMsg("NoResult") + "</div></td></tr>");
            }
        };

        var DeleteCardView = function () {
            var url = VIS.Application.contextUrl + "CardView/DeleteCardViewRecord";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ 'ad_CardView_ID': AD_CardView_ID }),
                success: function (data) {
                    var result = JSON.parse(data);
                    idx = cardsList.find('.crd-active').attr('idx');
                    if (cardViewInfo && cardViewInfo.length > 0) {
                        cardViewInfo.splice(idx, 1);
                    }
                    cardsList.find('.crd-active').remove();
                    if (cardViewInfo && cardViewInfo.length > 0) {
                        cardsList.find('div:first').addClass('crd-active');
                        idx = cardsList.find('.crd-active').attr('idx');
                        btnCancle.trigger('click');
                        AD_CardView_ID = cardViewInfo[idx].CardViewID;
                    } else {
                        AD_CardView_ID = 0;
                        idx = -1;
                        isSameUser = true;
                        btnNewCard.click();
                        btnCancle.addClass('vis-disable-event');
                    }
                    cardView.getCardViewData(mTab, AD_CardView_ID);
                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                }
            });
        };

        var ControlMgmt = function (indx) {
            if (indx && cardViewInfo && cardViewInfo[indx] && cardViewInfo[indx].CreatedBy == VIS.context.getAD_User_ID()) {
                isSameUser = true;
                //btnCopy.addClass('vis-disable-event');
                //btnEdit.removeClass('vis-disable-event');
                btnDelete.removeClass('vis-disable-event');

                btnSaveClose.removeClass('vis-disable-event');
                btnFinesh.removeClass('vis-disable-event');
                btnOnlySave.removeClass('vis-disable-event');
                btnChangeTemplate.removeClass('vis-disable-event');
            } else {
                isSameUser = false;
                //btnCopy.removeClass('vis-disable-event');
                //btnEdit.addClass('vis-disable-event');
                btnDelete.addClass('vis-disable-event');

                btnSaveClose.addClass('vis-disable-event');
                btnFinesh.addClass('vis-disable-event');
                btnOnlySave.addClass('vis-disable-event');
                btnChangeTemplate.addClass('vis-disable-event');
            }
            if (VIS.MRole.isAdministrator) {
                chkPublic.show();
            } else {
                chkPublic.hide();
            }
            //DivCradStep1.find('.vis-cr-right *:not(.vis-submit-win-btn *)').attr('disabled', 'disabled').css("pointer-events", "none");
            //DivCradStep1.find('.vis-sec-5').css("pointer-events", "auto");
            //DivCradStep1.find('.vis-submit-win-btn').css("pointer-events", "auto");
            //txtCardName.attr("disabled", "disabled");
        }

        var enableDisable = function (isEnable) {
            //if (isEnable) {
            //    DivCradStep1.find('.vis-cr-right *').removeAttr('disabled').css("pointer-events", "auto");;
            //    //availableFeilds.sortable('enable');
            //    //includedFeilds.sortable('enable');
            //    groupSequenceFeilds.sortable('enable');
            //    cardsList.find('div').css("pointer-events", "none");
            //    btnCancle.removeClass('vis-disable-event');
            //    btnEdit.addClass('vis-disable-event');
            //    btnNewCard.addClass('vis-disable-event');
            //    btnCopy.addClass('vis-disable-event');
            //    txtCardName.removeAttr('disabled');
            //} else {
            //    DivCradStep1.find('.vis-cr-right *:not(.vis-submit-win-btn *)').attr('disabled', 'disabled').css("pointer-events", "none");;
            //    //availableFeilds.sortable('disable');
            //    //includedFeilds.sortable('disable');
            //    groupSequenceFeilds.sortable('disable');
            //    btnCancle.addClass('vis-disable-event');

            //    isSameUser ? btnEdit.removeClass('vis-disable-event') : btnCopy.removeClass('vis-disable-event');
            //    btnNewCard.removeClass('vis-disable-event');
            //    cardsList.find('div').css("pointer-events", "auto");

            //    DivCradStep1.find('.vis-sec-5').css("pointer-events", "auto");
            //    DivCradStep1.find('.vis-submit-win-btn').css("pointer-events", "auto");
            //    txtCardName.attr("disabled", "disabled");
            //}           

            if (isNewRecord || isEdit) {
                //btnNewCard.addClass('vis-disable-event');
               // btnEdit.addClass('vis-disable-event');

               // btnCopy.addClass('vis-disable-event');

                isNewRecord ? btnDelete.addClass('vis-disable-event') : btnDelete.removeClass('vis-disable-event');

                if (isEdit && !isSameUser) {
                    btnDelete.addClass('vis-disable-event');
                }

            }
        }

        function SaveChanges(e) {
            IsBusy(true);
            window.setTimeout(function () {
                var cvConditionValue = "";
                var cvConditionText = "";
                strConditionArray = [];
                var queryValue = "";
                for (i = 0; i < cardviewCondition.length; i++) {
                    cvConditionValue = "";
                    cvConditionText = "";
                    for (j = 0; j < cardviewCondition[i].Condition.length; j++) {
                        if (j == 0) {
                            cvConditionValue += "@" + cardviewCondition[i].Condition[j].ColName + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryValue;
                            cvConditionText += "@" + cardviewCondition[i].Condition[j].ColHeader + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryText;
                        }
                        else {
                            cvConditionValue += " & " + "@" + cardviewCondition[i].Condition[j].ColName + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryValue;
                            cvConditionText += " & " + "@" + cardviewCondition[i].Condition[j].ColHeader + "@" + cardviewCondition[i].Condition[j].Operator + cardviewCondition[i].Condition[j].QueryText
                        }
                    }
                    strConditionArray.push({ "Color": cardviewCondition[i].Color.toString(), "ConditionValue": cvConditionValue, "ConditionText": cvConditionText })
                }


                if (isNewRecord) {
                    if (txtCardName.val() == "") {
                        VIS.ADialog.error("FillMandatory", true, "Name");
                        IsBusy(false);
                        return false;
                    }

                }
                else if (!isNewRecord && (AD_CardView_ID < 1 && VIS.MRole.isAdministrator)) {
                    VIS.ADialog.error("ClickNew", true, "");
                    IsBusy(false);
                    return false;
                }

                //var len = includedFeilds.find('div:not(:first)').length;
                //if (len.length <= 0)
                //    return false;

                SaveCardViewColumn();
                e.stopPropagation();
                e.preventDefault();
            }, 50);
        };

        var SaveCardViewColumn = function () {
            var idx = cardsList.find('.crd-active').attr('idx');
            if (idx && cardViewInfo != null && cardViewInfo.length > 0) {
                cardViewUserID = parseInt(cardViewInfo[idx].CreatedBy);
            } else {
                cardViewUserID = VIS.context.getAD_User_ID();
            }
            AD_User_ID = VIS.context.getAD_User_ID();

            if (isCopy) {
                isNewRecord = true;
                isCopy = false;
                if (!VIS.MRole.isAdministrator) {
                    chkPublic.prop("checked", false);
                    chkDefault.prop("checked", false);
                }
            }
            //if (VIS.context.getAD_User_ID() == cardViewUserID && !isNewRecord) {
            //    AD_User_ID = VIS.context.getAD_User_ID();
            //} else if (VIS.context.getAD_User_ID() != cardViewUserID && !isNewRecord && isEdit) {
            //    isNewRecord = true;
            //    AD_User_ID = VIS.context.getAD_User_ID();
            //    if (!VIS.MRole.isAdministrator) {
            //        chkPublic.prop("checked", false);
            //    }
            //} else {
            //    //  isNewRecord = true;
            //    AD_User_ID = VIS.context.getAD_User_ID();
            //}

            if (isNewRecord && cardViewInfo != null) {
                for (var a = 0; a < cardViewInfo.length; a++) {
                    if (cardViewInfo[a].CardViewName.trim() == txtCardName.val().trim()) {
                        VIS.ADialog.error("cardAlreadyExist", true, "");
                        IsBusy(false);
                        return false;
                    }
                }
            }

            cardViewName = txtCardName.val().trim();
            //}

            if (isEdit || isNewRecord) {
                SaveCardInfoFinal();
            }
            else {
                IsBusy(false);
                if (closeDialog) {
                    ch.close();
                    isEdit = false;
                    isNewRecord = false;
                    if (gc.isCardRow)
                        //cardView.requeryData();
                        cardView.getCardViewData(mTab, AD_CardView_ID);
                }
                else {
                    isEdit = true;
                    isNewRecord = false;
                    if (!isOnlySave) {
                        DivCradStep1.show();
                        DivCradStep2.hide();
                    }
                }
            }
        };

        var SaveCardInfoFinal = function () {
            //var len = includedFeilds.find('.vis-sec-2-sub-itm').length;
            //cardViewColArray = [];
            //var includeCols = [];
            //for (var i = 1; i < len; i++) {
                //var f = {};
                //f.AD_Field_ID = includedFeilds.find('.vis-sec-2-sub-itm').eq(i).attr("fieldid");
                //f.CardViewID = AD_CardView_ID;
                //f.sort = ulCardViewColumnField.children().eq(i).find('option:selected').val()
                //cardViewColArray.push(f);
                //includeCols.push(parseInt(f.AD_Field_ID));
            //}

            var grpSeq = "";
            var skipGrp = "";
            $.each(groupSequenceFeilds.find('.vis-sec-2-sub-itm'), function () {
                grpSeq += $(this).attr('key') + ",";
                if (!$(this).find('.grpChk').hasClass('fa-check-square-o')) {
                    skipGrp += $(this).attr('key') + ",";
                }
            });

            var selIdx = cardsList.find(".crd-active").attr('idx');
            grpSeq = grpSeq.replace(/,\s*$/, "");
            skipGrp = skipGrp.replace(/,\s*$/, "");

            var sortOrder = sortOrderArray.join(',');
            cardViewArray = [];
            var cardID = AD_CardView_ID;
            if (isNewRecord)
                cardID = 0;


            cardViewArray.push({
                AD_Window_ID: AD_Window_ID,
                AD_Tab_ID: AD_Tab_ID,
                UserID: AD_User_ID,
                AD_GroupField_ID: cmbGroupField.find(":selected").val(),
                isNewRecord: isNewRecord,
                CardViewName: cardViewName,
                CardViewID: cardID,
                IsDefault: chkDefault.is(":checked"),
                AD_HeaderLayout_ID: AD_HeaderLayout_ID,
                isPublic: chkPublic.is(":checked"),
                groupSequence: grpSeq
            });
           
            var url = VIS.Application.contextUrl + "CardView/SaveCardViewColumns";
            $.ajax({
                type: "POST",
                async: false,
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    'lstCardView': cardViewArray,
                    'lstCardViewColumns':cardViewColArray,
                    'lstCardViewCondition': strConditionArray,
                    'excludeGrp': skipGrp,
                    'orderByClause': sortOrder
                }),
                success: function (data) {
                    var result = JSON.parse(data);
                    AD_CardView_ID = result;
                    if (closeDialog) {
                        isNewRecord = false;
                        isEdit = false;
                        if (gc.isCardRow)
                            cardView.getCardViewData(mTab, AD_CardView_ID);
                        ch.close();
                    }
                    else {
                        if (isNewRecord) {
                            var idx = 0;
                            if (!cardViewInfo) {
                                cardViewInfo = [];
                            } else {
                                idx = cardViewInfo.length;
                            }
                            //<Option idx="+i+" is_shared=" + cardViewInfo[i].UserID + " ad_user_id=" + cardViewInfo[i].CreatedBy + " cardviewid=" + cardViewInfo[i].CardViewID + " groupSequence='" + cardViewInfo[i].groupSequence + "' excludedGroup='" + cardViewInfo[i].excludedGroup +"' ad_field_id=" + cardViewInfo[i].AD_GroupField_ID + " isdefault=" + cardViewInfo[i].DefaultID + " ad_headerLayout_id=" + cardViewInfo[i].AD_HeaderLayout_ID + "> " + w2utils.encodeTags(cardViewInfo[i].CardViewName) + "</Option>");
                            cardViewInfo.push({
                                'CardViewName': cardViewName,
                                'UserID': AD_User_ID,
                                'CreatedBy': VIS.context.getAD_User_ID(),
                                'CardViewID': AD_CardView_ID,
                                'groupSequence': grpSeq,
                                'excludedGroup': skipGrp,
                                'AD_GroupField_ID': cmbGroupField.find(":selected").val(),
                                'DefaultID': chkDefault.is(":checked"),
                                'AD_HeaderLayout_ID': AD_HeaderLayout_ID,
                                'OrderByClause': sortOrder,
                                'CreatedName': VIS.context.getAD_User_Name(),
                                'Updated': new Date().toLocaleDateString()
                            });

                            lastSelectedID = null;
                            cardsList.find('.crd-active').remove();
                            if (cardsList.find('.vis-lft-sgl').length > 0) {
                                var cardclone = cardsList.find('.vis-lft-sgl:first').clone(true);
                                cardclone.attr('idx', idx);
                                cardclone.addClass('crd-active');
                                cardclone.find('.vis-lft-sgl-title').text(w2utils.encodeTags(cardViewInfo[idx].CardViewName));
                                cardclone.find('.vis-lft-sgl-sub-title:first').text('Created By: '+ cardViewInfo[idx].CreatedName);
                                cardclone.find('.vis-lft-sgl-sub-title:last').text('last modified: ' + cardViewInfo[idx].Updated);
                                cardsList.prepend(cardclone);
                            } else {
                                var template = '<div idx="' + idx + '" class="vis-lft-sgl p-2 d-flex flex-column mb-2 crd-active">';
                                template += '<span class="vis-lft-sgl-title">' + w2utils.encodeTags(cardViewInfo[idx].CardViewName) + '</span>'
                                    + '    <span class="vis-lft-sgl-sub-title">Created By: ' + cardViewInfo[idx].CreatedName + '</span>'
                                    + '    <span class="vis-lft-sgl-sub-title">Last Modified: ' + cardViewInfo[idx].Updated + '</span>'
                                    + '</div>';
                                cardsList.prepend($(template));
                            }
                            

                            

                            //cmbCardView.append("<Option idx=" + idx + " is_shared=" + AD_User_ID + " ad_user_id=" + VIS.context.getAD_User_ID() + " cardviewid=" + AD_CardView_ID + " groupSequence='" + grpSeq + "' excludedGroup='" + skipGrp + "'  ad_field_id=" + cmbGroupField.find(":selected").attr("fieldid") + " isdefault=" + isdefault.is(":checked") + " ad_headerLayout_id=" + $vSearchHeaderLayout.getValue() + "> " + w2utils.encodeTags(cardViewName) + "</Option>");
                            //cmbCardView.find('[cardviewid="' + AD_CardView_ID + '"]').prop("selected", true).trigger("change");
                        }
                        else {
                            cardViewInfo[selIdx].groupSequence = grpSeq;
                            cardViewInfo[selIdx].excludedGroup = skipGrp;
                            cardViewInfo[selIdx].AD_GroupField_ID = cmbGroupField.find(":selected").attr("fieldid");
                            //cardViewInfo[selIdx].AD_HeaderLayout_ID = 0;
                            cardViewInfo[selIdx].CardViewName = cardViewName;
                            cardViewInfo[selIdx].UserID = AD_User_ID;
                            cardViewInfo[selIdx].OrderByClause = sortOrder;
                            cardViewInfo[selIdx].Updated = new Date().toLocaleDateString();
                            cardsList.find('.crd-active .vis-lft-sgl-title').text(cardViewName);
                        }
                        isEdit = true;
                        isNewRecord = false;
                        if (!isOnlySave) {
                            DivCradStep1.show();
                            DivCradStep2.hide();
                        }
                    }
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }
            });
            //return includeCols;
        };

        var resetAll = function () {
            txtCardName.val('');
            cmbGroupField.val('-1');
            cmbGroupField.change();
            cmbOrderClause.val('-1');
            sortOrderArray = [];
            lastSortOrderArray = [];
            LastCVCondition = [];
            //FillFields(false, false);
            //includedFeilds.find('.vis-sec-2-sub-itm:not(.displayNone)').remove();
            cardviewCondition = [];
            AddRow(cardviewCondition);
            clearOrderByClause();
            btnDelete.addClass('vis-disable-event');
            btnCancle.addClass('vis-disable-event');
            chkDefault.prop("checked", false);
            chkPublic.prop("checked", false);
            chkDefault.parent().show();
        }

        function getTargetMField(columnName) {
            // if no column name, then return null
            if (columnName == null || columnName.length == 0)
                return null;
            // else find field for the given column
            for (var c = 0; c < mTab.getFields().length; c++) {
                var field = mTab.getFields()[c];
                if (columnName.equals(field.getColumnName()))
                    return field;
            }
            return null;
        };

        function getControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                if (VIS.DisplayType.IsDate(crtlObj.getDisplayType())) {

                    var val = crtlObj.getValue();
                    if (val && val.endsWith('.000Z'))
                        val = val.replace('.000Z', 'Z');
                    return val;
                }

                if (VIS.DisplayType.IsNumeric(crtlObj.getDisplayType())) {
                   // return 0;
                }
                // return control's value
                if (crtlObj.getValue() == '') {
                    return null;
                }
                return crtlObj.getValue();
            }
            return "";
        };

        function SetControlValue(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // if control is any checkbox
                if (crtlObj.getDisplayType() == VIS.DisplayType.YesNo) {
                    if (crtlObj.getValue().toString().toLowerCase() == "true") {
                        return "Y";
                    }
                    else {
                        return "N";
                    }
                }
                // return control's value
                crtlObj.setValue(null);
            }

        };

        /* <param name="isValue1">true if get control's text at value1 position else false</param>
         */

        function getControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplay();
            }
            return "";
        };

        function SetControlText(isValue1) {
            var crtlObj = null;
            // get control
            if (isValue1) {
                // crtlObj = (IControl)tblpnlA2.GetControlFromPosition(2, 1);
                crtlObj = control1;
            }
            // if control exists
            if (crtlObj != null) {
                // get control's text
                return crtlObj.getDisplayType("");
            }
            return "";
        };

        function setControlNullValue(isValue2) {
            var crtlObj = null;
            if (isValue2) {
                crtlObj = control1;
            }

            // if control exists
            if (crtlObj != null) {
                crtlObj.setValue(null);
            }
        };

        function setValueEnabled(isEnabled) {
            // get control
            var ctrl = divValue1.children()[1];
            var btn = null;
            if (divValue1.children().length > 2)
                btn = divValue1.children()[2];

            if (btn)
                $(btn).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            else if (ctrl != null) {
                $(ctrl).prop("disabled", !isEnabled).prop("readonly", !isEnabled);
            }
        };

        function setEnableButton(btn, isEnable) {
            btn.prop("disabled", !isEnable);
        };

        function setControl(isValue1, field) {

            // set column and row position
            /*****Get control form specified column and row from Grid***********/
            if (isValue1)
                control1 = null;
            control2 = null;
            var ctrl = null;
            var ctrl2 = null;
            if (isValue1) {
                ctrl = divValue1.children()[0];
                if (divValue1.children().length > 1)
                    ctrl2 = divValue1.children()[1];
            }

            //Remove any elements in the list
            if (ctrl != null) {
                $(ctrl).remove();
                if (ctrl2 != null)
                    $(ctrl2).remove();
                ctrl = null;
            }
            /**********************************/
            var crt = null;
            // if any filed is given
            if (field != null) {
                // if field id any key, then show number textbox 
                if (field.getIsKey()) {
                    crt = new VIS.Controls.VNumTextBox(field.getColumnName(), false, false, true, field.getDisplayLength(), field.getFieldLength(),
                        field.getColumnName());
                }
                else {
                    crt = VIS.VControlFactory.getControl(null, field, true, true, false);
                }
            }
            else {
                // if no field is given show an empty disabled textbox
                crt = new VIS.Controls.VTextBox("columnName", false, true, false, 20, 20, "format",
                    "GetObscureType", false);// VAdvantage.Controls.VTextBox.TextType.Text, DisplayType.String);
            }

            if (crt != null) {
                //crt.SetIsMandatory(false);
                crt.setReadOnly(false);

                if (VIS.DisplayType.Text == field.getDisplayType() || VIS.DisplayType.TextLong == field.getDisplayType()) {
                    crt.getControl().attr("rows", "1");
                    crt.getControl().css("width", "100%");
                }
                else if (VIS.DisplayType.YesNo == field.getDisplayType()) {
                    crt.getControl().css("clear", "both");
                }
                else if (VIS.DisplayType.IsDate(field.getDisplayType())) {
                    crt.getControl().css("line-height", "1");
                }

                var btn = null;
                if (crt.getBtnCount() > 0 && !(crt instanceof VIS.Controls.VComboBox))
                    btn = crt.getBtn(0);

                if (isValue1) {
                    divValue1.find('label').remove();
                    divValue1.append(crt.getControl().css("width", "95%"));
                    control1 = crt;
                    if (btn) {
                        divValue1.append(btn);
                        //crt.getControl().css("width", "60%");
                        btn.css("max-width", "35px");
                    }
                    if (VIS.DisplayType.YesNo != field.getDisplayType()) {
                        divValue1.append('<label for="txtQueryValue">' + VIS.Msg.getMsg("QueryValue") + '</label>');
                    }
                }
            }
        };

        function addOrderByClauseFromDB(OrderByClause) {
            clearOrderByClause();
            if (OrderByClause && OrderByClause.length > 0) {
                sortOrderArray = OrderByClause.split(",");
                for (var m = 0; m < sortOrderArray.length; m++) {
                    var val = sortOrderArray[m].split(' ');
                    var f = mTab.getField(val[0]);
                    addOrderByClauseItems(f.getHeader(), val[0], val[1]);
                }
            }
        };

        function clearOrderByClause() {
            sortList.empty();
            sortOrderArray = [];
            lastSortOrderArray = [];
        };

        function addOrderByClauseItems(selectedColtext, val, isAsc) {
            var item = '<div class="vis-sortListItem">'
                + '<p>' + selectedColtext + '</p>'
                + '<div class="vis-sortListIcons">'
                + '<span class="vis-sortAsc">';
            if (isAsc == "ASC")
                item += '<i class="fa fa-sort-amount-asc"></i>';
            else
                item += '<i class="fa fa-sort-amount-asc" style="transform: rotate(180deg);padding-top:1px"></i>';
            item += '</span>'
                + '<span class="vis-sortIcon vis-sortListItemClose" data-text="' + val + ' ' + isAsc + '">'
                + '<i class="fa fa-close"></i>'
                + '</span>'
                + '</div>'
                + '</div>';
            // $divHeadderLay.append('<label>' + selectedColtext + '(' + isAsc + ')</label>');
            sortList.append(item);
        };

        function addSelectedTemplate() {
            var $this = DivTemplate.find('.vis-active-template').clone(true);
            if ($this.attr("lastupdated")) {
                spnLastSaved.text(VIS.Msg.getMsg("LastSaved") + " " + $this.attr("lastupdated"));
            }
            $this.find('.fieldValue').remove();
            CardCreatedby = $this.attr("createdBy");
            isSystemTemplate = $this.attr("isSystemTemplate");
            AD_HeaderLayout_ID = $this.find('.mainTemplate').attr('templateid');
            templateName = $this.find('.mainTemplate').attr('name');
            if (AD_HeaderLayout_ID == "0") {

                $this.find('.mainTemplate').html($('<div sectionCount="1" class="section1 vis-wizard-section" style="padding:5px;"></div>'));
            }
            //txtTemplateName.val($(this).find('.mainTemplate').attr('name')).attr("templateid", $(this).find('.mainTemplate').attr('templateid'));

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

        // #endregion

        // #region Step 2 events
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

        var events2 = function () {

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
                } else if (cmd == 'Hide') {
                    if (blok.prop('tagName') == 'I') {
                        blok.attr("class", "");
                        blok.next().attr('showFieldIcon', true);
                    } else if (blok.prop('tagName') == 'IMG') {
                        blok.addClass('displayNone');
                        f.attr('showFieldIcon', true);
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
                } else if (cmd == 'Unlink') {
                    unlinkField(blok.attr('title'), blok);
                } else if (cmd == 'ShowImg') {
                    blok.prev('img').removeClass("displayNone");
                    f.attr('showFieldIcon', false);
                    divTopNavigator.find('[command="ShowImg"]').parent().hide();
                } else if (cmd == 'ShowValue') {
                    blok.next('.fieldValue').css("display", "unset");
                    divTopNavigator.find('[command="ShowValue"]').parent().hide();
                }

                if (isChange && AD_HeaderLayout_ID !="0") {
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
                    divTopNavigator.find('[command="Unlink"]').parent().hide();
                    divTopNavigator.find('[command="fieldName"]').text('').hide();
                    if ($(e.target).hasClass('fieldLbl')) {
                        divTopNavigator.find('[command="fieldName"]').text($(e.target).closest('.fieldGroup').find('.fieldLbl').attr('title')).show();
                        divTopNavigator.find('[command="Unlink"]').parent().show();
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
                            var displayType = mTab.getFieldById(Number(target.attr('fieldid'))).getDisplayType();
                            if (displayType == VIS.DisplayType.TableDir || displayType == VIS.DisplayType.Table || displayType == VIS.DisplayType.List) {
                                if ($(e.target).prev('img:hidden').length > 0) {
                                    divTopNavigator.find('[command="ShowImg"]').parent().show();
                                    divTopNavigator.find('[command="Hide"]').parent().hide();
                                } else if ($(e.target).next('.fieldValue:hidden').length > 0) {
                                    divTopNavigator.find('[command="Hide"]').parent().hide();
                                    divTopNavigator.find('[command="ShowValue"]').parent().show();
                                } else {
                                    divTopNavigator.find('[command="Hide"]').parent().show();
                                }
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
                gridCss();
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
            });

            DivGridSec.find('.grdColDel').click(function () {
                var idx = $(this).closest('.colBox').index();
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1; 
                for (var i = 0; i < totalRow; i++) {
                    var dNo = idx + (i * totalCol) - 1;
                    activeSection.find('.grdDiv').eq(dNo).addClass('del');
                    var blk = activeSection.find('.grdDiv').eq(i - 1).find('.fieldLbl');                    
                    if (blk && blk.attr('title') && blk.attr('title').length > 0) {
                        unlinkField(blk.attr('title'), blk);
                    }
                }
                activeSection.find('.del').remove();
                $(this).closest('.colBox').remove();               
                gridCss();
                isChange = true;
                if (isChange && AD_HeaderLayout_ID != "0") {
                    btn_BlockCancel.show();
                }
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

            DivCardField.find('.vis-grey-icon .fa-circle').click(function () {
                if ($(this).hasClass('vis-succes-clr')) {
                    var fid = $(this).closest('.fieldLbl').attr('fieldid');
                    DivViewBlock.find('[fieldid="' + fid + '"]').mousedown().mouseup();
                    setTimeout(function () {
                        divTopNavigator.find('[command="Unlink"]').click();        
                    },100);
                               
                } else {
                    linkField($(this).closest('.fieldLbl'));
                }
            });

            btn_BlockCancel.click(function () {
                DivTemplate.find('.mainTemplate[templateid="' + AD_HeaderLayout_ID + '"]').parent().click();  
                if (count > 1) {
                    fillcardLayoutfromTemplate();
                }

                btn_BlockCancel.hide();
            });
        }      
        // #endregion
        // #region Step 2 functions    

        function gridCss() {            
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

            grSec.find('.grdDiv').each(function (index) {
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

            //grSec.css({
            //    'grid-template-columns': 'repeat(' + totalCol + ',1fr)',
            //    'grid-template-rows': 'repeat(' + totalRow + ',1fr)',
            //    'gap': txtRowGap.val() + 'px ' + txtColGap.val() + 'px'
            //});

            if (totalCol > 0 && totalRow > 0) {
                var oldrow = grSec.attr('row');
                var oldcol = grSec.attr('col');
                if (oldcol != totalCol) {
                    for (var r = 0; r < oldrow; r++) {
                        var pos = oldcol * r + (r + 1);                        
                        grSec.find('.grdDiv').eq(pos).after("<div class='grdDiv' style='padding:10px;'></div>");
                    }

                } else {
                    var totalDiv = totalRow * totalCol - grSec.find('.grdDiv').length;
                    for (var i = 0; i < totalDiv; i++) {
                        grSec.append("<div class='grdDiv' style='padding:10px;'></div>");
                    }
                }

                //grSec.attr('row', totalRow);
               // grSec.attr('col', totalCol);

                grSec.find('.grdDiv').unbind('mouseover');
                grSec.find('.grdDiv').mouseover(function (e) {
                    if (mdown && ($(this).find('.vis-split-cell').length == 0)) {
                        selectTo($(this));
                    }

                });
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
                        rClone =DivGridSec.find('.rowBox:last').clone(true);
                    } else if (key.indexOf('col_') != -1) {
                        cClone.find('input').val(item.val);
                        cClone.find('select').val(item.msr);
                        DivGridSec.find('.DivColBox').append(cClone);
                        cClone =DivGridSec.find('.colBox:last').clone(true);
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
            var colBoxLen = DivGridSec.find('.colBox').length-1;
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
                styleValue = 'linear-gradient' + styleValue ;
            }

            if (commend == 'boxShadow') {
                var x = DivStyleSec1.find('.boxX').val();
                var y = DivStyleSec1.find('.boxY').val();
                var b = DivStyleSec1.find('.boxB').val();
                var c = DivStyleSec1.find('.boxC').val();
                styleValue = x + ' ' + y + ' ' + b + ' ' + c;
            }

            if (editorProp[commend].proprty == 'justify-content' || editorProp[commend].proprty == "align-items") {
                tag.css("display", "flex");
            }

            tag.css(editorProp[commend].proprty, $.trim(styleValue));
        }

        function getTemplateDesign() {
            var url = VIS.Application.contextUrl + "CardView/getTemplateDesign";
            var obj = {
                ad_Window_ID: mTab.getAD_Window_ID(),
                ad_Tab_ID: mTab.getAD_Tab_ID()
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
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }, complete: function () {
                    DivTemplate.find('.vis-cardSingleViewTemplate').click(function () {
                        DivTemplate.find('.vis-cardSingleViewTemplate').removeClass('vis-active-template');
                        $(this).addClass('vis-active-template');                        
                    });
                }
            });
        }

        function saveTemplate(e) {
            IsBusy(true);
            //if (txtTemplateName.val() == "") {
            //    VIS.ADialog.error("FillMandatory", true, "Template Name");
            //    return false;
            //}
            var fieldObj = [];
            var seq = 10;
            var cardSection = [];

            cardViewColArray = [];
            //var includeCols = [];
            //for (var i = 1; i < len; i++) {
                //var f = {};
                //f.AD_Field_ID = includedFeilds.find('.vis-sec-2-sub-itm').eq(i).attr("fieldid");
                //f.CardViewID = AD_CardView_ID;
                //f.sort = ulCardViewColumnField.children().eq(i).find('option:selected').val()
                //cardViewColArray.push(f);
                //includeCols.push(parseInt(f.AD_Field_ID));
            //}

            DivViewBlock.find('.fieldGroup:not(:hidden)').each(function (index) {
                var gridArea = $(this).closest('.grdDiv').css('grid-area').split('/');
                var secNo = $(this).closest('.vis-wizard-section').attr("sectioncount");
                gridObj['section' + secNo]["style"] = $(this).closest('.vis-wizard-section').attr("style");
                gridObj['section' + secNo]["sectionID"] = $(this).closest('.vis-wizard-section').attr("sectionid") || 0;

                var valueStyle = "";
                if ($(this).find('img').length > 0) {
                    valueStyle = '@value::' + $(this).find('.fieldValue').attr('style') || 'display:none';
                    valueStyle += ' |@img::' + $(this).find('img').attr('style') || '';

                } else {
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
                    hideFieldIcon = $(this).find('fieldLbl').attr('showfieldicon') == 'true' ? true : false;                }
               

                obj1 = {
                    cardFieldID: $(this).attr('cardfieldid'),
                    sectionNo: secNo*10,
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
                    columnSQL: columnSQL
                }

                var f = {}
                f.AD_Field_ID = obj1.fieldID;
                f.CardViewID = AD_CardView_ID;
                cardViewColArray.push(f);
                seq += 10;
                fieldObj.push(obj1);
            });
            //console.log(gridObj);
            //console.log(fieldObj);
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

            if (isSystemTemplate == "Y") {
                templateName = (templateName || txtCardName.val()) + "_" + Math.floor(Math.random() * 10000);
                AD_HeaderLayout_ID = 0;
            };

            if (isCopy) {
                templateName = (txtCardName.val() + "_" + Math.floor(Math.random() * 10000));
                AD_HeaderLayout_ID = 0;
                AD_CardView_ID = "undefined"; 
                isNewRecord=true
            }
            

            var cardID = AD_CardView_ID;
            if (isNewRecord)
                cardID = 0;
            var finalobj = {
                CardViewID: cardID,
                templateID: AD_HeaderLayout_ID || 0,
                templateName: templateName,
                style: DivViewBlock.find('.vis-viewBlock').attr('style'),
                cardSection: cardSection,
                cardTempField: fieldObj
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
                    isSystemTemplate = 'N';
                    if (!isNewRecord) {
                        isEdit = true;
                    }
                    SaveChanges(e)
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }
            });

            
        }

        function fill(htm) {
            DivStyleSec1.find('#master001_' + WindowNo + ' input').val('');
            DivStyleSec1.find('#master001_' + WindowNo + ' select').val('');
            DivStyleSec1.find('.gradient1').val('#833ab4');
            DivStyleSec1.find('.gradient2').val('#fcb045');
            DivStyleSec1.find("[data-command1]").parent().removeClass('vis-hr-elm-inn-active');
            var styles = htm.attr('style');
            if (htm.find('sql').length > 0) {
                txtSQLQuery.val(VIS.secureEngine.decrypt(htm.find('sql').attr("query")));
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
            //itm.removeAttr('class').removeAttr('showfieldtext').removeAttr('showfieldicon');
            itm.next().remove();
            if (itm.prev().prop('tagName') == 'I') {
                itm.prev().remove();
            }

            var fld = DivCardField.find('[fieldid="' + itm.attr('fieldid') + '"]');
            fld.find('.linked').removeClass('linked vis-succes-clr');
            fld.removeAttr('seqNo');
           
            fld.prop("draggable", true);
            var listItem = DivCardField.find('.fieldLbl:not(.displayNone)').not('[seqNo]');
            var sortedList = [];
            listItem.each(function (i, item) {
                sortedList.push($.trim($(this).text().toLowerCase()));
            });

            var initialLength = sortedList.length;
            sortedList.push($.trim(fieldName.toLowerCase()));
            sortedList.sort();

            ////var iclone = DivCardField.find('fields:first').clone(true);
            var i = $.inArray($.trim(fieldName.toLowerCase()), sortedList);
            var linkedLength = DivCardField.find('.linked').length;
            i += linkedLength;


            ////fld.find('.fa-link').addClass('fa-chain-broken').removeClass('fa-link vis-succes-clr');          
            if (i == initialLength) {
                DivCardField.append(fld);
            } else {
                DivCardField.find('.fieldLbl').eq(i + 1).after(fld);
            }
            itm.remove();
            divTopNavigator.hide();
        }

        function linkField(itm) {
            var blok = DivViewBlock.find('.vis-active-block');
            if (blok.hasClass('grdDiv')) {
                var fieldHtml = $('<div class="fieldGroup"></div>');                 
                var fID = itm.attr('fieldid');
                var newitm = itm.clone(true);
                newitm.attr("showfieldicon", true);
                itm.find('.vis-grey-icon').remove();
                itm.attr('draggable',false);
                if (mTab.getFieldById(Number(fID)).getShowIcon()) {                   
                    fieldHtml.append($('<i class="fa fa-star">&nbsp;</i>'));                  
                }

                blok.append(fieldHtml);
                fieldHtml.append(itm);
                var displayType = mTab.getFieldById(Number(fID)).getDisplayType();
                var src = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' width='50' height='50'%3E%3Cdefs%3E%3Cpath d='M23 31l-3.97-2.9L19 28l-.24-.09.19.13L13 33v2h24v-2l-3-9-5-3-6 10zm-2-12c0-1.66-1.34-3-3-3s-3 1.34-3 3 1.34 3 3 3 3-1.34 3-3zm-11-8c-.55 0-1 .45-1 1v26c0 .55.45 1 1 1h30c.55 0 1-.45 1-1V12c0-.55-.45-1-1-1H10zm28 26H12c-.55 0-1-.45-1-1V14c0-.55.45-1 1-1h26c.55 0 1 .45 1 1v22c-.3.67-.63 1-1 1z' id='a'/%3E%3C/defs%3E%3Cuse xlink:href='%23a' fill='%23fff'/%3E%3Cuse xlink:href='%23a' fill-opacity='0' stroke='%23000' stroke-opacity='0'/%3E%3C/svg%3E";
                   
                if (displayType == VIS.DisplayType.Image) {
                     fieldHtml.append($('<img class="vis-colorInvert" src="' + src + '"/>'));
                } else if (displayType == VIS.DisplayType.TableDir || displayType == VIS.DisplayType.Table || displayType == VIS.DisplayType.List) {
                    fieldHtml.append($('<img class="vis-colorInvert displayNone" src="' + src + '"/>'));
                    fieldHtml.append($('<span class="fieldValue">:Value</span>')); 
                } else {
                    fieldHtml.append($('<span class="fieldValue">:Value</span>'));
                }

                var linkedLength = DivCardField.find('.linked').length;
                
                newitm.find('.fa-circle').addClass('linked vis-succes-clr');
                newitm.prop("draggable", false);
                //newitm.find('.fa-chain-broken').addClass('vis-succes-clr').removeClass('fa-chain-broken');
                newitm.attr('seqNo', fieldHtml.attr('seqNo') || (linkedLength*10));
                DivCardField.find('.fieldLbl').eq(linkedLength).after(newitm);
            }
            isChange = true;
            if (isChange && AD_HeaderLayout_ID != "0") {
                btn_BlockCancel.show();
            }
        }

        function saveCopyCard(copyCardName) {
            for (var a = 0; a < cardViewInfo.length; a++) {
                if (cardViewInfo[a].CardViewName.trim() == copyCardName.trim()) {
                    VIS.ADialog.error("cardAlreadyExist", true, "");
                    IsBusy(false);
                    return false;
                }
            }

            txtCardName.val(copyCardName);
            isCopy = true;
            btnCardCustomization.click();
            //setTimeout(function () {
            //    btnFinesh.click();
            //}, 1000);


        }
        // #endregion
        init();
       
        this.getRoot = function () {
            return root;
        };

        this.show = function () {
            var w = $(window).width() - 150;
            var h = $(window).height() - 10;
            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("Card"));
            ch.setWidth(w);
            ch.setHeight(h);
            ch.setContent(root);
            ch.onOkClick = function (e) {
            }    
            ch.show();
            ch.hideButtons();
        }
       
    };


    cvd.prototype.getOperatorsQuery = function (vnpObj, translate) {
        var html = "";
        var val = "";
        for (var p in vnpObj) {
            val = vnpObj[p];
            if (translate)
                val = VIS.Msg.getMsg(val);
            html += '<option value="' + p + '">' + val + '</option>';
        }
        return html;
    }
    cvd.prototype.sizeChanged = function (height, width) {
        console.log(height, width);
    }
    VIS.CVDialog = cvd;

    function cardCopyDialog() {
        var $root = $('<div class="input-group vis-input-wrap mb-0" >');
        var $controlBlock = $('<div class="vis-control-wrap d-block mt-1" >');
        var txtDescription = $('<span style="display:block;margin-bottom:5px;">' + VIS.Msg.getMsg('NewCardInfo') + '</span>');
        $root.append(txtDescription).append($controlBlock);
        var $txtName = $('<input type="text">');
        var $lblName = $('<label>' + VIS.Msg.getMsg('EnterName') + '</label>');
        $controlBlock.append($txtName).append($lblName);   
       
       
        var self = this;
        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("CardName"));
            ch.setModal(true);
            ch.setContent($root);
            ch.setWidth("50%");
            ch.show();
            ch.onOkClick = ok;
            ch.onCancelClick = cancel;
            ch.onClose = cancel;
        };

        this.getName = function () {
            return $txtName.val();
        };

        function ok() {
            if ($txtName.val() == null || $txtName.val() == "") {
                VIS.ADialog.info("FileNameMendatory");
                return false;
            }
            self.onSave();
            return true;
        };

        function cancel() {
            ch.close();
            return true;
        };


        function dispose() {
            $txtName.remove();
            $txtName = null;
            txtDescription.remove();
            txtDescription = null;
            $lblName.remove();
            $lblName = null;
            $root.remove();
            $root = null;
            ch = null;
        };
    };

    VIS.CardCopyDialog = cardCopyDialog;


}(VIS, jQuery));