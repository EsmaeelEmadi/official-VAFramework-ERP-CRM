﻿; (function (VIS, $) {

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
        var btnSaveNext = null;
        var btnSaveClose = null;
        var DivCradStep1 = null;
        var DivCradStep2 = null;
        var btnNext = null;
        var btnBack = null;
        var btnFinesh = null;
        var count = 1;
        var LstCardViewCondition = null;
        var dbResult = null;
        var cardViewInfo = [];
        var cardsList = null;
        var txtCardName = null;
        var cmbColumn = null;
        var drpOp = null;
        var cmbGroupField = null;
        var availableFeilds = null;
        var includedFeilds = null;
        var groupSequenceFeilds = null;
        var cmbOrderClause = null;
        var sortList = null;
        var isAsc = "ASC";
        var btnCopy = null;
        var btnEdit = null;
        var btnDelete = null;
        var btnCancle = null;
        var btnNewCard = null;
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
        var txtTemplateName = null;
        var sectionCount = 2;
        var AD_HeaderLayout_ID = 0;
        var templateID = 0;
        var templateName = null;
        var txtCustomStyle = null;
        var CardCreatedby = null;
        var hideShowGridSec = null;
        var gridObj = {
        };
        function init() {
            root = $('<div style="width:1148px"></div>');
            isBusyRoot = $("<div class='vis-apanel-busy vis-cardviewmainbusy'></div> ");
            root.append(isBusyRoot);
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
            btnSaveNext.click(function (e) {
                closeDialog = false;
                //var len = includedFeilds.find('.vis-sec-2-sub-itm').length;
                //var iClone = DivCardField.find('fields:first').clone(true);
                //iClone.removeClass('displayNone');
                //for (var i = 1; i < len; i++) {
                //    var incld = includedFeilds.find('.vis-sec-2-sub-itm').eq(i);
                //    iClone.text(incld.find('.vis-sub-itm-title').text());
                //    iClone.attr("fieldid", incld.attr("fieldid")).attr("seqNo",i*10);
                //    iClone.attr("id", WindowNo + "_" + incld.attr("fieldid"));
                //    DivCardField.append(iClone);
                //    iClone = DivCardField.find('fields:first').clone(true);
                //    iClone.removeClass('displayNone');
                //}

                SaveChanges(e);                
                DivTemplate.find('.mainTemplate[templateid="' + AD_HeaderLayout_ID + '"]').parent().click();
                
            });
            btnSaveClose.click(function (e) {
                closeDialog = true;
                SaveChanges(e);
            });
            btnNewCard.click(function () {
                isEdit = false;
                isNewRecord = true;
                resetAll();
                enableDisable(true);
                lastSelectedID = cardsList.find('.crd-active').attr('idx');
                cardsList.find('.crd-active').removeClass('crd-active');
                template = '<div class="vis-lft-sgl p-2 d-flex flex-column my-2 crd-active">';
                template += '<span class="vis-lft-sgl-title">--</span>'
                    + '    <span class="vis-lft-sgl-sub-title">Created By: ' + VIS.context.getAD_User_Name() + '</span>'
                    + '    <span class="vis-lft-sgl-sub-title">Last Modified: ' + new Date().toLocaleDateString() + '</span>'
                    + '</div>';

                cardsList.prepend($(template));

            });

            btnEdit.click(function () {
                isEdit = true;
                isNewRecord = false;
                enableDisable(true);
                chkDefault.parent().hide();
            });

            btnCopy.click(function () {
                isEdit = true;
                isNewRecord = false;
                enableDisable(true);
                chkDefault.parent().hide();
            });

            btnCancle.click(function () {
                isNewRecord = false;
                isEdit = false;
                enableDisable(false);
                if (lastSelectedID) {
                    cardsList.find('.crd-active').remove();
                    cardsList.find("[idx='" + lastSelectedID + "']").addClass('crd-active');
                    lastSelectedID = null;
                }
                cardsList.find('.crd-active').trigger('click');
            });

            btnNext.click(function () {
                //if (txtTemplateName.val() == "" || txtTemplateName.val() == null) {
                //    VIS.ADialog.error("FillMandatory", true, "Template Name");
                //    return false;
                //}
                if (count == 1) {
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
                                    msr: 'fr'
                                }
                            };

                            for (var j = 0; j < totalCol; j++) {
                                Obj['col_' + j] = {
                                    val: 1,
                                    msr: 'fr'
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

                    DivCardField.find('fields[seqNo]').each(function (i) {
                        var fID = $(this).attr('fieldid');
                        var vlu = $(this).text();
                        var fidItm = DivViewBlock.find('[seqNo="' + $(this).attr('seqNo') + '"]');
                        if (fidItm.length>0) {
                            fidItm.append($('<fields draggable="true" ondragstart="drag(event)" fieldid="' + fID + '" id="' + $(this).attr('id') + '">' + vlu + '</fields><fieldvalue style="' + fidItm.attr("fieldValuestyle") + '">:Value</fieldvalue>'));
                            $(this).remove();
                        }
                    });                   
                    
                }
                count++;
                DivTemplate.hide();
                DivViewBlock.find('fields').show();
                DivViewBlock.find('fieldvalue').show();
                DivCardFieldSec.show();
                hideShowGridSec.show();
            });


            btnFinesh.click(function () {
                saveTemplate();
            });

            btnBack.click(function () {
                count--;
                DivTemplate.show();
                DivViewBlock.find('fields').hide();
                DivViewBlock.find('fieldvalue').hide();
                DivCardFieldSec.hide();
                hideShowGridSec.hide();
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
                    txtTemplateName.val(cardViewInfo[idx].CardViewName);
                    AD_CardView_ID = cardViewInfo[idx].CardViewID;
                    cardViewUserID = cardViewInfo[idx].CreatedBy;
                    chkDefault.prop("checked", cardViewInfo[idx].DefaultID ? true : false);
                    chkPublic.prop("checked", cardViewInfo[idx].UserID > 0 ? false : true);
                    AD_HeaderLayout_ID = cardViewInfo[idx].AD_HeaderLayout_ID;
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
                var $slider = DivGrdntBlock.find('.vis-circular-slider-dot')
                var deg = getGradientDeg($slider, e);
                $slider.css({ WebkitTransform: 'rotate(' + deg + 'deg)' });
                $slider.css({ '-moz-transform': 'rotate(' + deg + 'deg)' });
                $slider.attr("deg", deg);
                updateGradientColor();
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
                });
            }

            txtCustomStyle.change(function () {
                var selectedItem = DivViewBlock.find('.vis-active-block');
                selectedItem.attr("style", $(this).val());
            });

            /* End Step 1*/

        }

        // #endregion

        function CardViewUI() {
            root.load(VIS.Application.contextUrl + 'CardViewWizard/Index/?windowno=' + WindowNo, function (event) {
                /*step 1*/
                DivCradStep1 = root.find('#DivCardStep1_' + WindowNo);
                btnSaveNext = root.find('#btnSaveNextStep1_' + WindowNo);
                btnSaveClose = root.find('#btnSaveCloseStep1_' + WindowNo);
                btnNext = root.find('#BtnNext_' + WindowNo);
                btnBack = root.find('#BtnBack' + WindowNo);
                btnFinesh = root.find('#BtnFinesh_' + WindowNo);
                cardsList = root.find('#DivCardList_' + WindowNo);
                txtCardName = root.find('#txtCardName_' + WindowNo);
                cmbGroupField = root.find('#cmbGroupField_' + WindowNo);
                availableFeilds = root.find('#AvailableFeilds_' + WindowNo);
                includedFeilds = root.find('#IncludedFeilds_' + WindowNo);
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
                btnEdit = root.find('#btnEdit_' + WindowNo);
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
                hideShowGridSec = root.find('.DivGridSec');
                DivTemplate = root.find('#DivTemplate_' + WindowNo);
                DivCardField = root.find('#DivCardField_' + WindowNo);
                txtTemplateName = root.find('#txtTemplateName_' + WindowNo);
                DivCardFieldSec = root.find('#DivCardFieldSec_' + WindowNo);
                txtCustomStyle = root.find('#txtCustomStyle_' + WindowNo);
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
                cmbOrderClause.append('<option value="-1"> </option>)');

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
                                template = '<div idx="' + i + '" class="vis-lft-sgl p-2 d-flex flex-column my-2 crd-active">';
                            } else {
                                template = '<div idx="' + i + '" class="vis-lft-sgl p-2 d-flex flex-column my-2">';
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
                        txtTemplateName.val(cardViewInfo[idx].CardViewName);
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

        var FillFields = function (isReload, isShowAllColumn) {
            //if (!isReload) {
            var feildClone = availableFeilds.find('.vis-sec-2-sub-itm:first').clone(true);
            availableFeilds.find('.vis-sec-2-sub-itm').remove();

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

                var iClone = DivCardField.find('fields:first').clone(true);
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

                    feildClone.find('.vis-sub-itm-title').text(tabField[i].getHeader());
                    feildClone.attr("fieldid", tabField[i].getAD_Field_ID());
                    availableFeilds.append(feildClone);
                    feildClone = availableFeilds.find('.vis-sec-2-sub-itm:first').clone(true);

                    iClone.text(tabField[i].getHeader());
                    iClone.attr("fieldid", tabField[i].getAD_Field_ID());
                    iClone.attr("id", WindowNo + "_" + tabField[i].getAD_Field_ID());
                    DivCardField.append(iClone);
                    iClone = DivCardField.find('fields:first').clone(true);
                    iClone.removeClass('displayNone');

                    //availableFeilds.append("<li index=" + i + " FieldID=" + tabField[i].getAD_Field_ID() + "> <span>" + tabField[i].getHeader() + "</span></li>");
                }
            }

            availableFeilds.sortable({
                connectWith: ".connectedSortable",
                disabled: true
            }).disableSelection();
            includedFeilds.sortable({
                connectWith: ".connectedSortable",
                disabled: true
            }).disableSelection();
        };

        var FillIncluded = function (isReload) {
            var IncldfeildClone = includedFeilds.find('.displayNone').clone(true);
            IncldfeildClone.removeClass('displayNone');
            includedFeilds.find('.vis-sec-2-sub-itm:not(.displayNone)').remove();  
            
            var iClone = DivCardField.find('fields:first').clone(true);
            DivCardField.find('fields:not(:first)').remove();
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
                                IncldfeildClone.find('.vis-sub-itm-title').text(CVColumns[i].FieldName);
                                IncldfeildClone.attr("fieldid", CVColumns[i].AD_Field_ID);
                                includedFeilds.append(IncldfeildClone);
                                IncldfeildClone = includedFeilds.find('.vis-sec-2-sub-itm:last').clone(true);


                                iClone.text(CVColumns[i].FieldName);
                                iClone.attr("fieldid", CVColumns[i].AD_Field_ID).attr("seqNo", CVColumns[i].SeqNo);
                                iClone.attr("id", WindowNo + "_" + CVColumns[i].AD_Field_ID);
                                DivCardField.append(iClone);
                                iClone = DivCardField.find('fields:first').clone(true);
                                iClone.removeClass('displayNone');

                                // ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + CVColumns[i].AD_Field_ID + "> <span>" + CVColumns[i].FieldName + "</span></li>");

                            }
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
                        IncldfeildClone.find('.vis-sub-itm-title').text(includedFields[i].getHeader());
                        IncldfeildClone.attr("fieldid", includedFields[i].AD_Field_ID);
                        includedFeilds.append(IncldfeildClone);
                        IncldfeildClone = includedFeilds.find('.vis-sec-2-sub-itm:last').clone(true);

                        iClone.text(includedFields[i].getHeader());
                        iClone.attr("fieldid", includedFields[i].AD_Field_ID).attr("seqNo", includedFields[i].SeqNo);
                        iClone.attr("id", WindowNo + "_" + includedFields[i].AD_Field_ID);
                        DivCardField.append(iClone);
                        iClone = DivCardField.find('fields:first').clone(true);
                        iClone.removeClass('displayNone');

                        //ulRoot.append("<li seqno=" + 0 + " index=" + i + " CardViewColumnID=" + 0 + " FieldID=" + includedFields[i].getAD_Field_ID() + "> " + includedFields[i].getHeader() + "</li>");
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
                            //item.find('input').prop('checked', false);
                        }

                        var before = groupSequenceFeilds.find(".vis-sec-2-sub-itm").eq(j);
                        item.insertBefore(before);
                    }
                }

            } else {
                //ulGroupSeqColumns.parent().css("background-color", "rgba(var(--v-c-on-secondary), 0.04)");
                groupSequenceFeilds.append('<div class="onlyLOV" style="padding-top:40%;text-align:center" key="">' + VIS.Msg.getMsg("OnlyForLOV") + '</div>');
            }
            groupSequenceFeilds.sortable({
                disabled: true
            });
        }

        var FillCVConditionCmbColumn = function () {
            var html = '<option value="-1"> </option>';
            for (var c = 0; c < findFields.length; c++) {
                // get field
                var field = findFields[c];
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
            if (data != null) {
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
                btnCopy.addClass('vis-disable-event');
                btnEdit.removeClass('vis-disable-event');
                btnDelete.removeClass('vis-disable-event');
            } else {
                isSameUser = false;
                btnCopy.removeClass('vis-disable-event');
                btnEdit.addClass('vis-disable-event');
                btnDelete.addClass('vis-disable-event');
            }
            if (VIS.MRole.isAdministrator) {
                chkPublic.show();
            } else {
                chkPublic.hide();
            }
            DivCradStep1.find('.vis-cr-right *:not(.vis-submit-win-btn *)').attr('disabled', 'disabled').css("pointer-events", "none");
            DivCradStep1.find('.vis-sec-5').css("pointer-events", "auto");
            DivCradStep1.find('.vis-submit-win-btn').css("pointer-events", "auto");
        }

        var enableDisable = function (isEnable) {
            if (isEnable) {
                DivCradStep1.find('.vis-cr-right *').removeAttr('disabled').css("pointer-events", "auto");;
                availableFeilds.sortable('enable');
                includedFeilds.sortable('enable');
                groupSequenceFeilds.sortable('enable');
                cardsList.find('div').css("pointer-events", "none");
                btnCancle.removeClass('vis-disable-event');
                btnEdit.addClass('vis-disable-event');
                btnNewCard.addClass('vis-disable-event');
                btnCopy.addClass('vis-disable-event');
            } else {
                DivCradStep1.find('.vis-cr-right *:not(.vis-submit-win-btn *)').attr('disabled', 'disabled').css("pointer-events", "none");;
                availableFeilds.sortable('disable');
                includedFeilds.sortable('disable');
                groupSequenceFeilds.sortable('disable');
                btnCancle.addClass('vis-disable-event');

                isSameUser ? btnEdit.removeClass('vis-disable-event') : btnCopy.removeClass('vis-disable-event');
                btnNewCard.removeClass('vis-disable-event');
                cardsList.find('div').css("pointer-events", "auto");

                DivCradStep1.find('.vis-sec-5').css("pointer-events", "auto");
                DivCradStep1.find('.vis-submit-win-btn').css("pointer-events", "auto");
            }

            if (isNewRecord || isEdit) {
                btnNewCard.addClass('vis-disable-event');
                btnEdit.addClass('vis-disable-event');

                btnCopy.addClass('vis-disable-event');

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

                var len = includedFeilds.find('div:not(:first)').length;
                if (len.length <= 0)
                    return false;

                SaveCardViewColumn(cardViewColArray);
                e.stopPropagation();
                e.preventDefault();
            }, 50);
        };

        var SaveCardViewColumn = function (lstCardView) {
            var idx = cardsList.find('.crd-active').attr('idx');
            if (idx && cardViewInfo != null && cardViewInfo.length > 0) {
                cardViewUserID = parseInt(cardViewInfo[idx].CreatedBy);
            } else {
                cardViewUserID = VIS.context.getAD_User_ID();
            }

            if (VIS.context.getAD_User_ID() == cardViewUserID && !isNewRecord) {
                AD_User_ID = VIS.context.getAD_User_ID();
            } else if (VIS.context.getAD_User_ID() != cardViewUserID && !isNewRecord && isEdit) {
                isNewRecord = true;
                AD_User_ID = VIS.context.getAD_User_ID();
                if (!VIS.MRole.isAdministrator) {
                    chkPublic.prop("checked", false);
                }
            } else {
                //  isNewRecord = true;
                AD_User_ID = VIS.context.getAD_User_ID();
            }

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
                    if (gc.isCardRow)
                        //cardView.requeryData();
                        cardView.getCardViewData(mTab, AD_CardView_ID);
                }
                else {
                    btnCancle.trigger("click");
                    DivCradStep1.hide();
                    DivCradStep2.show();
                }
            }
        };

        var SaveCardInfoFinal = function () {
            var len = includedFeilds.find('.vis-sec-2-sub-itm').length;
            cardViewColArray = [];
            var includeCols = [];
            for (var i = 1; i < len; i++) {
                var f = {};
                f.AD_Field_ID = includedFeilds.find('.vis-sec-2-sub-itm').eq(i).attr("fieldid");
                f.CardViewID = AD_CardView_ID;
                //f.sort = ulCardViewColumnField.children().eq(i).find('option:selected').val()
                cardViewColArray.push(f);
                includeCols.push(parseInt(f.AD_Field_ID));
            }

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
                AD_HeaderLayout_ID: isEdit ? cardViewInfo[selIdx].AD_HeaderLayout_ID : 0,
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
                    'lstCardViewColumns': cardViewColArray,
                    'lstCardViewCondition': strConditionArray,
                    'excludeGrp': skipGrp,
                    'orderByClause': sortOrder
                }),
                success: function (data) {
                    var result = JSON.parse(data);
                    AD_CardView_ID = result;
                    if (closeDialog) {
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
                                'AD_HeaderLayout_ID': 0,
                                'OrderByClause': sortOrder,
                                'CreatedName': VIS.context.getAD_User_Name(),
                                'Updated': new Date().toLocaleDateString()
                            });
                            lastSelectedID = null;
                            cardsList.find('.crd-active').remove();
                            var template = '<div idx="' + idx + '" class="vis-lft-sgl p-2 d-flex flex-column my-2 crd-active">';
                            template += '<span class="vis-lft-sgl-title">' + w2utils.encodeTags(cardViewInfo[idx].CardViewName) + '</span>'
                                + '    <span class="vis-lft-sgl-sub-title">Created By: ' + cardViewInfo[idx].CreatedName + '</span>'
                                + '    <span class="vis-lft-sgl-sub-title">Last Modified: ' + cardViewInfo[idx].Updated + '</span>'
                                + '</div>';

                            cardsList.prepend($(template));

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
                        btnCancle.trigger('click');
                        DivCradStep1.hide();
                        DivCradStep2.show();
                    }
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }
            });
            return includeCols;
        };

        var resetAll = function () {
            txtCardName.val('');
            cmbGroupField.val('-1');
            cmbGroupField.change();
            cmbOrderClause.val('-1');
            sortOrderArray = [];
            lastSortOrderArray = [];
            LastCVCondition = [];
            FillFields(false, true);
            includedFeilds.find('.vis-sec-2-sub-itm:not(.displayNone)').remove();
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
                    return 0;
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
                    divValue1.append('<label for="txtQueryValue">' + VIS.Msg.getMsg("QueryValue") + '</label>');
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



        // #endregion

        // #region Step 2 events
        var measurment = ['px', '%', 'cm', 'mm', 'in', 'pc', 'pt', 'ch', 'em', 'rem', 'vh', 'vw', 'vmin', 'vmax'];
        var editorProp = {
            width: {
                proprty: 'width',
                value: '',
                measurment: true
            },
            height: {
                proprty: 'height',
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
                var $slider = DivStyleSec1.find('.vis-circular-slider-dot')
                var deg = getGradientDeg($slider, e);
                $slider.css({ WebkitTransform: 'rotate(' + deg + 'deg)' });
                $slider.css({ '-moz-transform': 'rotate(' + deg + 'deg)' });
                $slider.attr("deg", deg);
                applyCommend("gradient", deg);
            });

            var viewBlock = DivViewBlock.find('*');

            viewBlock.mousedown(function (e) {
                DivViewBlock.find('.vis-active-block').removeClass('vis-active-block');
                if (count == 1) {
                    DivViewBlock.find('.vis-viewBlock').addClass("vis-active-block");
                } else {
                    $(e.target).addClass("vis-active-block");
                    var secCount = $(e.target).closest('.vis-wizard-section').attr("sectionCount");
                    if (!DivGridSec.find("[sectionCount='" + secCount + "']").hasClass('section-active')) {
                        DivGridSec.find("[sectionCount='" + secCount + "'] .vis-grey-disp-el").click();
                    }


                    if ($(e.target).find('.vis-split-cell').length == 0) {
                        mdown = true;
                        mdown = true;
                        var totalCol = DivGridSec.find('.colBox').length - 1;
                        activeSection.find('.grdDiv').each(function (e) {
                            var currentRow = Math.ceil((e + 1) / totalCol);
                            if ($(this).hasClass('vis-active-block')) {
                                startRowIndex = currentRow - 1;
                                startCellIndex = (e) - totalCol * (startRowIndex);
                            }
                        });
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
                        $(this).addClass('vis-editor-validate');
                        return;
                    } else if (isNaN(Number(mvalue))) {
                        $(this).addClass('vis-editor-validate');
                        return;
                    }
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

            });

            DivGridSec.find('.addGridRow').click(function () {
                var rClone = DivGridSec.find('.rowBox:first').clone(true);
                rClone.show();
                DivGridSec.find('.DivRowBox').append(rClone);
                createGrid();
            });

            DivGridSec.find('.addGridCol').click(function () {
                var cClone = DivGridSec.find('.colBox:first').clone(true);
                cClone.show();
                DivGridSec.find('.DivColBox').append(cClone);
                createGrid();
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

            });

            DivGridSec.find('.grid-Section .vis-grey-disp-el-xross').click(function () {
                var secNo = $(this).closest('.grid-Section').attr('sectionCount');
                if ($(this).closest('.grid-Section').hasClass('section-active')) {
                    DivGridSec.find('.grid-Section').eq(1).addClass('section-active');
                    DivGridSec.find('.grid-Section').eq(1).find('.vis-grey-disp-el').click();
                }
                $(this).closest('.grid-Section').remove();
                DivViewBlock.find('.vis-viewBlock .section' + secNo).remove();
                secNo = DivGridSec.find('.section-active').attr('sectionCount');
                activeSection = DivViewBlock.find('.vis-viewBlock .section' + secNo);

            });

            DivGridSec.find('.grid-Section .vis-grey-disp-el').click(function () {
                DivGridSec.find('.section-active').removeClass('section-active');
                $(this).parent().addClass('section-active');
                var secNo = DivGridSec.find('.section-active').attr('sectionCount');

                activeSection = DivViewBlock.find('.vis-viewBlock .section' + secNo);
                createGridRowCol();
            });

            DivGridSec.find('.grdRowDel').click(function () {
                var idx = $(this).closest('.rowBox').index();
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1;                
                var dNo = (idx * totalCol + 1) - totalCol;
                for (var i = dNo; i < (dNo + totalCol); i++) {
                    activeSection.find('.grdDiv').eq(i - 1).addClass('del');
                }
                activeSection.find('.del').remove();
                $(this).closest('.rowBox').remove();
                gridCss();
            });

            DivGridSec.find('.grdColDel').click(function () {
                var idx = $(this).closest('.colBox').index();
                var totalRow = DivGridSec.find('.rowBox').length - 1;
                var totalCol = DivGridSec.find('.colBox').length - 1; 
                for (var i = 0; i < totalRow; i++) {
                    var dNo = idx + (i * totalCol) - 1;
                    activeSection.find('.grdDiv').eq(dNo).addClass('del');
                }
                activeSection.find('.del').remove();
                $(this).closest('.colBox').remove();
                gridCss();
            });

            DivGridSec.find('.mergeCell').click(function () {
                mergeCell();
            });

            txtColGap.change(function () {
                gridCss();
            });

            txtRowGap.change(function () {
                gridCss();
            });

            DivGridSec.find('.rowBox input,select').change(function () {
                gridCss();
            });

            DivGridSec.find('.colBox input,select').change(function () {
                gridCss();
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
                rowCss += $(this).find('input').val() + '' + $(this).find('select :selected').val() + ' ';
                Obj['row_' + i] = {
                    val: $(this).find('input').val(),
                    msr: $(this).find('select :selected').val(),                    
                }
            });
            var colCss = "";
            DivGridSec.find('.colBox:not(:first)').each(function (i) {
                colCss += $(this).find('input').val() + '' + $(this).find('select :selected').val() + ' ';
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
                var totalDiv = totalRow * totalCol - grSec.find('.grdDiv').length;
                for (var i = 0; i < totalDiv; i++) {
                    grSec.append("<div class='grdDiv'></div>");
                }
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

            var unMearge = $('<span class="vis-split-cell">x</span>');
            activeSection.find('.vis-active-block:not(:first)').hide().removeClass('vis-active-block');
            activeSection.find('.vis-active-block:first').css('grid-area', rowStart + '/' + colStart + '/' + rowEnd + '/' + colEnd).append(unMearge);

            unMearge.click(function () {
                var gArea = $(this).parent().css('grid-area').split('/');
                var totalCol = DivGridSec.find('.colBox').length - 1;
                var DVB = activeSection.find('.grdDiv');
                for (var i = Number($.trim(gArea[0])); i < Number($.trim(gArea[2])); i++) {
                    for (var j = Number($.trim(gArea[1])); j < Number($.trim(gArea[3])); j++) {
                        var gIdx = totalCol * (i - 1) + (j - 1);
                        DVB.eq(gIdx)[0].style.removeProperty('grid-area');
                        DVB.eq(gIdx)[0].style.removeProperty('display');
                    }
                }
                $(this).remove();
                gridCss();
            });
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

        function applyCommend(commend,styleValue) {
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
            $.ajax({
                type: "POST",
                url: url,
                dataType: "json",
                contentType: 'application/json; charset=utf-8',                
                success: function (data) {
                    var result = JSON.parse(data);
                    DivTemplate.find('.vis-template-zone').append(result);
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }, complete: function () {
                    DivTemplate.find('.vis-template-single').click(function () {
                        FillFields(false, false);
                        CardCreatedby = $(this).attr("createdBy");
                         templateID = $(this).find('.mainTemplate').attr('templateid');
                         templateName = $(this).find('.mainTemplate').attr('name');
                        //txtTemplateName.val($(this).find('.mainTemplate').attr('name')).attr("templateid", $(this).find('.mainTemplate').attr('templateid'));

                        if ($(this).html() != "" || $(this).html() != null) {
                            $(this).find('.vis-wizard-section').each(function () {
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
                                        $(this).append('<span class="vis-split-cell">x</span>');
                                    }
                                    arr[idx] = $(this)[0].outerHTML;
                                });
                                $(this).html(arr.join(" "));
                            });
                            DivViewBlock.find('.grdDiv').unbind('mouseover');
                            DivViewBlock.find('.vis-viewBlock').attr("style", $(this).find('.mainTemplate').attr('style'));
                            DivViewBlock.find('.vis-viewBlock').html($(this).find('.mainTemplate').html());                            
                            DivViewBlock.find('.grdDiv').mouseover(function (e) {
                                if (mdown && ($(this).find('.vis-split-cell').length == 0)) {
                                    selectTo($(this));
                                }
                            });
                        }

                        
                    });
                }
            });
        }

        function saveTemplate() {
            //if (txtTemplateName.val() == "") {
            //    VIS.ADialog.error("FillMandatory", true, "Template Name");
            //    return false;
            //}
            var fieldObj = [];
            var seq = 10;
            var cardSection = [];

            DivViewBlock.find('.grdDiv:not(:hidden)').each(function (index) {
                var gridArea = $(this).css('grid-area').split('/');
                var secNo = $(this).closest('.vis-wizard-section').attr("sectioncount");
                gridObj['section' + secNo]["style"] = $(this).closest('.vis-wizard-section').attr("style");
                gridObj['section' + secNo]["sectionID"] = $(this).closest('.vis-wizard-section').attr("sectionid")||0;

                obj1 = {
                    cardFieldID: $(this).attr('cardfieldid'),
                    sectionNo: secNo,
                    rowStart: $.trim(gridArea[0]),
                    rowEnd: $.trim(gridArea[2]),
                    colStart: $.trim(gridArea[1]),
                    colEnd: $.trim(gridArea[3]),
                    seq: seq,                   
                    style: $(this).attr('style'),
                    fieldID: $(this).find('fields').attr('fieldid'),
                    fieldStyle: $(this).find('fields').attr('style') || '',
                    valueStyle: $(this).find('fieldvalue').attr('style') || ''
                }

                seq += 10;
                fieldObj.push(obj1);
            });
            //console.log(gridObj);
            //console.log(fieldObj);
            Object.keys(gridObj).forEach(function (key) {
                var fobj = {
                    sectionName: 'section ' + gridObj[key].sectionNo,
                    sectionID: gridObj[key].sectionID,
                    sectionNo: gridObj[key].sectionNo,
                    style: gridObj[key].style,
                    totalRow: gridObj[key].totalRow,
                    totalCol: gridObj[key].totalCol,
                    rowGap: gridObj[key].rowGap,
                    colGap: gridObj[key].colGap
                };
                cardSection.push(fobj);
            });

            if (VIS.context.getAD_User_ID() !== CardCreatedby) {
                templateName = (templateName || txtCardName.val()) + "_" + Math.floor(Math.random() * 10000);
            };

            var cardID = AD_CardView_ID;
            if (isNewRecord)
                cardID = 0;
            var finalobj = {
                CardViewID: cardID,
                templateID: templateID || 0,
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
                    IsBusy(false);

                }, error: function (errorThrown) {
                    alert(errorThrown.statusText);
                    IsBusy(false);
                }
            });

            
        }

        function fill(htm) {
            DivStyleSec1.find('.tab-pane input:not(:hidden)').val('');
            var styles = htm.attr('style');
            txtCustomStyle.val(styles);
            styles && styles.split(";").forEach(function (e) {
                var style = e.split(":");
                for (const a in editorProp) {
                    if ($.trim(style[0]) == $.trim(editorProp[a].proprty) && editorProp[a].value == '') {
                        var v = $.trim(style[1]);
                        //if (DivStyleSec1.find("[data-command='" + a + "']").attr('type') == 'color') {
                        //    //v = rgb2hex(v);
                        //    //DivStyleSec1.find("[data-command='" + a + "']").val(v);
                        //    //DivStyleSec1.find("[data-command='" + a + "']").change();
                        //} else {
                        //}

                        DivStyleSec1.find("[data-command='" + a + "']").val(v);
                        
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
        // #endregion
        init();
       
        this.getRoot = function () {
            return root;
        };

        this.show = function () {
            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("Card"));
            //ch.setWidth("90%");
            //ch.setHeight("90%");
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

    VIS.CVDialog = cvd;

    function cardCopyDialog() {
        var $root = $('<div style="padding-bottom:0px">');
        var txtDescription = $('<span style="display:block;margin-bottom:5px;">' + VIS.Msg.getMsg('NewCardInfo') + '</span>');
        var $txtName = $('<input style="margin-left:5px">');
        var $lblName = $('<label>' + VIS.Msg.getMsg('EnterName') + '</label>');
        $root.append(txtDescription).append($lblName).append($txtName);
        var self = this;
        this.show = function () {

            ch = new VIS.ChildDialog();
            ch.setTitle(VIS.Msg.getMsg("CardName"));
            ch.setModal(true);
            ch.setContent($root);
            ch.setWidth("90%");
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
            self.onClose();
            return true;
        };

        function cancel() {
            self.onClose();
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

    //alert(VIS.CVDialog);

}(VIS, jQuery));