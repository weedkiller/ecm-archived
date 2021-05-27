//v.3.5 build 120731

/*
Copyright DHTMLX LTD. http://www.dhtmlx.com
To use this component please contact sales@dhtmlx.com to obtain license
*/
dhtmlXGridObject.prototype._process_json_row=function(a,b){a._attrs=b;for(var f=0;f<a.childNodes.length;f++)a.childNodes[f]._attrs={};if(b.userdata)for(var h in b.userdata)this.setUserData(a.idd,h,b.userdata[h]);for(var b=this._c_order?this._swapColumns(b.data):b.data,e=0;e<b.length;e++)if(typeof b[e]=="object"&&b[e]!=null){a.childNodes[e]._attrs=b[e];if(b[e].type)a.childNodes[e]._cellType=b[e].type;b[e]=b[e].value}this._fillRow(a,b);return a};
dhtmlXGridObject.prototype._process_js_row=function(a,b){a._attrs=b;for(var f=0;f<a.childNodes.length;f++)a.childNodes[f]._attrs={};if(b.userdata)for(var h in b.userdata)this.setUserData(a.idd,h,b.userdata[h]);for(var e=[],c=0;c<this.columnIds.length;c++){e[c]=b[this.columnIds[c]];if(typeof e[c]=="object"&&e[c]!=null){a.childNodes[c]._attrs=e[c];if(e[c].type)a.childNodes[c]._cellType=e[c].type;e[c]=e[c].value}!e[c]&&e[c]!==0&&(e[c]="")}this._fillRow(a,e);return a};
dhtmlXGridObject.prototype.updateFromJSON=function(a,b,f,h){typeof b=="undefined"&&(b=!0);this._refresh_mode=[!0,b,f];this.load(a,h,"json")};
dhtmlXGridObject.prototype._refreshFromJSON=function(a){this._f_rowsBuffer&&this.filterBy(0,"");reset=!1;if(window.eXcell_tree)eXcell_tree.prototype.setValueX=eXcell_tree.prototype.setValue,eXcell_tree.prototype.setValue=function(a){var b=this.grid._h2.get[this.cell.parentNode.idd];b&&this.cell.parentNode.valTag?this.setLabel(a):this.setValueX(a)};var b=this.cellType._dhx_find("tree"),f=a.parent||0,h={};this._refresh_mode[2]&&(b!=-1?this._h2.forEachChild(f,function(a){h[a.id]=!0},this):this.forEachRow(function(a){h[a]=
!0}));for(var e=a.rows,c=0;c<e.length;c++){var g=e[c],d=g.id;h[d]=!1;if(this.rowsAr[d]&&this.rowsAr[d].tagName!="TR")this._h2?this._h2.get[d].buff.data=g:this.rowsBuffer[this.getRowIndex(d)].data=g,this.rowsAr[d]=g;else if(this.rowsAr[d])this._process_json_row(this.rowsAr[d],g,-1),this._postRowProcessing(this.rowsAr[d],!0);else if(this._refresh_mode[1]){var j={idd:d,data:g,_parser:this._process_json_row,_locator:this._get_json_data},i=this.rowsBuffer.length;this._refresh_mode[1]=="top"?(this.rowsBuffer.unshift(j),
i=0):this.rowsBuffer.push(j);if(this._h2)reset=!0,this._h2.add(d,f).buff=this.rowsBuffer[this.rowsBuffer.length-1];this.rowsAr[d]=g;g=this.render_row(i);this._insertRowAt(g,i?-1:0)}}if(this._refresh_mode[2])for(d in h)h[d]&&this.rowsAr[d]&&this.deleteRow(d);this._refresh_mode=null;if(window.eXcell_tree)eXcell_tree.prototype.setValue=eXcell_tree.prototype.setValueX;reset&&this._renderSort();if(this._f_rowsBuffer)this._f_rowsBuffer=null,this.filterByAll()};
dhtmlXGridObject.prototype._process_js=function(a){return this._process_json(a,"js")};
dhtmlXGridObject.prototype._process_json=function(a,b){this._parsing=!0;try{if(a&&a.xmlDoc)eval("dhtmlx.temp="+a.xmlDoc.responseText+";"),a=dhtmlx.temp;else if(typeof a=="string")eval("dhtmlx.temp="+a+";"),a=dhtmlx.temp}catch(f){dhtmlxError.throwError("LoadXML","Incorrect JSON",[a.xmlDoc||a,this]),a={rows:[]}}if(this._refresh_mode)return this._refreshFromJSON(a);var h=parseInt(a.pos||0),e=parseInt(a.total_count||0),c=!1;e&&(this.rowsBuffer[e-1]||(this.rowsBuffer.length&&(c=!0),this.rowsBuffer[e-1]=
null),e<this.rowsBuffer.length&&(this.rowsBuffer.splice(e,this.rowsBuffer.length-e),c=!0));for(var g in a)g!="rows"&&this.setUserData("",g,a[g]);if(this.isTreeGrid())return this._process_tree_json(a,null,null,b);if(b=="js"){if(a.data)a=a.data;for(var d=0;d<a.length;d++)if(!this.rowsBuffer[d+h]){var j=a[d],i=j.id||d+1;this.rowsBuffer[d+h]={idd:i,data:j,_parser:this._process_js_row,_locator:this._get_js_data};this.rowsAr[i]=a[d]}}else for(d=0;d<a.rows.length;d++)if(!this.rowsBuffer[d+h])i=a.rows[d].id,
this.rowsBuffer[d+h]={idd:i,data:a.rows[d],_parser:this._process_json_row,_locator:this._get_json_data},this.rowsAr[i]=a.rows[d];if(c&&this._srnd){var k=this.objBox.scrollTop;this._reset_view();this.objBox.scrollTop=k}else this.render_dataset();this._parsing=!1};dhtmlXGridObject.prototype._get_json_data=function(a,b){return typeof a.data[b]=="object"?a.data[b].value:a.data[b]};
dhtmlXGridObject.prototype._process_tree_json=function(a,b,f,h){this._parsing=!0;var e=!1;if(!b){this.render_row=this.render_row_tree;e=!0;b=a;f=b.parent||0;f=="0"&&(f=0);if(!this._h2)this._h2=new dhtmlxHierarchy;if(this._fake)this._fake._h2=this._h2}if(h=="js"){if(b.data&&!f)a=b.data;if(b.rows)b=b.rows;for(var c=0;c<b.length;c++){var g=b[c].id,d=this._h2.add(g,f);d.buff={idd:g,data:b[c],_parser:this._process_js_row,_locator:this._get_js_data};if(b[c].open)d.state="minus";this.rowsAr[g]=d.buff;this._process_tree_json(b[c],
b[c],g,h)}}else if(b.rows)for(c=0;c<b.rows.length;c++){g=b.rows[c].id;d=this._h2.add(g,f);d.buff={idd:g,data:b.rows[c],_parser:this._process_json_row,_locator:this._get_json_data};if(b.rows[c].open)d.state="minus";this.rowsAr[g]=d.buff;this._process_tree_json(b.rows[c],b.rows[c],g,h)}if(e)f!=0&&this._h2.change(f,"state","minus"),this._updateTGRState(this._h2.get[f]),this._h2_to_buff(),f!=0&&(this._srnd||this.pagingOn)?this._renderSort():this.render_dataset(),this._slowParse===!1&&this.forEachRow(function(a){this.render_row_tree(0,
a)}),this._parsing=!1};

//v.3.5 build 120731

/*
Copyright DHTMLX LTD. http://www.dhtmlx.com
To use this component please contact sales@dhtmlx.com to obtain license
*/