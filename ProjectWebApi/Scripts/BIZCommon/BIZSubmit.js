var ElementTypes = { Form: "FORM", LinkedTable: "LINKEDTABLE" };

function FormInfo(formName) {
   this.formName = formName;
   this.Type = ElementTypes.Form;
}

function LinkedTableInfo(gridView) {
   this.gridView = gridView;
   this.Type = ElementTypes.LinkedTable;
}

function isEmptyOrSpaces(str) {
   return str === null || str.match(/^ *$/) !== null;
}

String.prototype.attachParameter = function attachParameter(name, value) {
   var parameterStr = name + '=' + value;
   if (isEmptyOrSpaces(this)) return parameterStr;
   else return this + '&' + parameterStr;
}

String.prototype.attachGrid = function attachGrid(grid) {
   var value = getGridParamValue(grid);
   if (!value) return this;
   return this.attachParameter(getGridParamName(grid), value);
}

String.prototype.attachForm = function attachForm(form) {
   var formStr = form.serialize();
   if (isEmptyOrSpaces(this)) return formStr;
   else return this + '&' + formStr;
}

function getGridParamName(grid) {
	return grid.name + '_eLeedBatchUpdateEditState';
}

function getGridParamValue(grid) {
	var helper = grid.GetBatchEditHelper();
	if (!helper) return null;
	var editState = helper.GetEditState();
   if (!editState) return null;
   return JSON.stringify(editState);
}

function collectParameters(forms, grids) {
   var result = '';
   if (Array.isArray(forms)) {
      for (var i = 0; i < forms.length; i++) {
         result = result.attachForm(forms[i]);
      }
   }
   if (Array.isArray(grids))
      for (var j = 0; j < grids.length; j++) {
         result = result.attachGrid(grids[j]);
      }
   return result;
}

function submitData(actionUrl, forms, grids, success, error, complete, addData) {
   var url = actionUrl;
   var data = collectParameters(forms, grids);
   if (addData) data = data + '&' + addData;
   $.ajax({
      async: false,
      url: url,
      type: 'post',
      data: data,
      xhrFields: { withCredentials: true },
      success: function (data, textStatus, jqXHR) {
         if (success) success(data, textStatus, jqXHR);
      },
      error: function (jqXHR, textStatus, errorThrown) {
         if (error) error(jqXHR, textStatus, errorThrown);
      },
      complete: function (jqXHR, textStatus) {
         if (complete) complete(jqXHR, textStatus);
      }
   });
}

//Multirequests

function finishEleedThread(controllerPath, elementsToDisable, commit) {
   $.ajax({
      cache: false,
      type: 'POST',
      url: controllerPath + 'FinishThread',
      data: { commit: commit },
      xhrFields: { withCredentials: true },
      success: function (data) { },
      complete: function () {
         delete $.ajaxSettings.headers['eLeedThread'];
         if (Array.isArray(elementsToDisable)) {
            elementsToDisable.forEach(function (element) {
               if (element.SetEnabled) element.SetEnabled(true);
            });
            lt_messsagesGrid.Refresh();
         }
      }
   });
}

function submitElements(elements, controllerPath, elementsToDisable) {
   if (Array.isArray(elements) && elements.length > 0) {
      if (controllerPath[controllerPath.length - 1] !== '/') controllerPath = controllerPath + '/';

      if (Array.isArray(elementsToDisable)) {
         elementsToDisable.forEach(function (element) {
            if (element.SetEnabled) element.SetEnabled(false);
         });
      }

      $.ajax({
         cache: false,
         type: 'POST',
         url: controllerPath + 'InitializeThread',
         xhrFields: { withCredentials: true },
         success: function (data) {
            var threadId = data;
            $.ajaxSetup({ headers: { 'eLeedThread': threadId } });

            processElements(elements, controllerPath, elementsToDisable);
         }
      });

   }
}

function processElements(elements, controllerPath, elementsToDisable) {
   if (elements.length > 0) {
      var curElement = elements[0];

      if (curElement != undefined && curElement.Type === ElementTypes.Form) {
         $.ajax({
            cache: false,
            type: 'POST',
            url: $('#' + curElement.formName).attr('action'),
            data: $('#' + +curElement.formName).serialize(),
            xhrFields: { withCredentials: true },
            success: function (data) {
               //TODO подумать над ошибками валидации
               alert(curElement.Type);
               processElements(elements.slice(1, elements.length), controllerPath, elementsToDisable);
            },
            error: function () {
               //TODO подумать над поведением при ошибках
               finishEleedThread(controllerPath, elementsToDisable, false);
            }
         });
      }
      else if (curElement != undefined && curElement.Type === ElementTypes.LinkedTable && curElement.gridView.batchEditApi.HasChanges()) {
         var isError = false;
         var callbackErrorMeth = function (sender, args) {
            alert('Error while updating Table ' + sender.name + '\n' + args.message);
            args.handled = true;
            isError = true;
            //TODO подумать над поведением при ошибках
            finishEleedThread(controllerPath, elementsToDisable, false);
            curElement.gridView.CallbackError.RemoveHandler(callbackErrorMeth);
         }

         var endCallbackMeth = function (sender, args) {
            //TODO Подумать над ошибками валидации
            alert(curElement.Type);
            sender.applyCallbackResult = false;
            if (!isError)
               processElements(elements.slice(1, elements.length), controllerPath, elementsToDisable);
            //alert('UPDATEEDIT CALLED ' + sender.name);
            curElement.gridView.CallbackError.RemoveHandler(callbackErrorMeth);
            curElement.gridView.EndCallback.RemoveHandler(endCallbackMeth);
         };

         curElement.gridView.EndCallback.AddHandler(endCallbackMeth);
         curElement.gridView.CallbackError.AddHandler(callbackErrorMeth);
         curElement.gridView.SetEnabled(true);
         curElement.gridView.UpdateEdit();
      }
      else {
         processElements(elements.slice(1, elements.length), controllerPath, elementsToDisable);
      }
   } else {
      finishEleedThread(controllerPath, elementsToDisable, true);
   }
}


function FilterSubLinkedTable(gridParent, gridChild, gridChildName) {
    var data = gridParent.cpData;
    if (data) {
        if (gridParent.GetFocusedRowIndex() != -1) {
            gridChild.AutoFilterByColumn(gridChildName, data[gridParent.GetFocusedRowIndex()]);
        }
    }
}

function FillValue(gridParent, gridChild, gridChildName, curentEditingIndex) {
    var data = gridParent.cpData;
    if (data) {
        if (gridParent.GetFocusedRowIndex() != -1) {
            var uid = data[gridParent.GetFocusedRowIndex()];
            //alert(uid + ' - ' + curentEditingIndex + ' 88 ' + gridChild.GetFocusedRowIndex());
            gridChild.batchEditApi.SetCellValue(curentEditingIndex, gridChildName, uid);
        }
    }
}

function ResizeSplitterPaneControl(control, splitterPane, onlyHorizontal, onlyVertical) {
	if (control) {
      if (onlyVertical === false && splitterPane)
			control.SetWidth(splitterPane.GetClientWidth());
      if (onlyHorizontal === false && splitterPane)
			control.SetHeight(splitterPane.GetClientHeight());
	}
}

function RenumberBatchGridRows(grid, fieldName, deletedIndex) {
	var rowVisibleIndices = grid.batchEditApi.GetRowVisibleIndices(false);
	var curIndex = 1;
	for (var i = 0; i < rowVisibleIndices.length; i++) {
		if (rowVisibleIndices[i] === deletedIndex) continue;
		grid.batchEditApi.SetCellValue(rowVisibleIndices[i], fieldName, curIndex++, null, true);
   }
	var helper = grid.batchEditApi.GetHelper();
	if (helper) helper.updatedValues = {};
}

function GetMaxBatchgridFieldValue(grid, fieldName) {
	var returnFieldValue = 0;

	var rowVisibleIndices = grid.batchEditApi.GetRowVisibleIndices(false);
	for (var i = 0; i < rowVisibleIndices.length; i++) {
		var fieldValue = grid.batchEditApi.GetCellValue(rowVisibleIndices[i], fieldName);
		if (fieldValue > returnFieldValue) {
			returnFieldValue = fieldValue;
		}
	}

	return returnFieldValue;
}  