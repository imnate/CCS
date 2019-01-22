<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="teacher.aspx.cs" Inherits="OSMVA_UploadHW.teacher" MasterPageFile="~/MainFrame.Master" %>

<asp:Content ID="MainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    
    <%----%>
    <link rel="stylesheet" href="bootstrap-3.3.7-dist/css/menu-scroll.css" />
    <nav class=" navbar navbar-default navbar-fixed-top">
        <div class="row">
            <div class="col-md-2">
            </div>
            <%--            var datas = new Array();
            .each(
                datas.push(parseInt(data.num));

            )

            data: datas;--%>

            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand" href="#">線上程式批改系統</a>
                    <button type="button" class="btn btn-default navbar-btn" data-toggle="modal" data-target="#Announce_from" data-whatever="@fat">發布公告</button>
                </div>
                <div class="nav navbar-nav">
                    <ul class="nav navbar-nav">
                        <li class="dropdown">
                            <a href="#" class="glyphicon glyphicon-search dropdown-toggle" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">查詢<span class="caret"></span></a>
                            <ul class="dropdown-menu">
                                <li><a id="show_hwstatus" href="#" data-toggle="modal" data-target="#check_hwlist">查詢新增作業</a></li>
                                <li><a id="charts" href="#" data-toggle="modal" data-target="#show_charts">統計圖表</a></li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </nav>

    <br />
    <br />
    <br />
    <div class="modal fade" id="show_charts" tabindex="-1" role="dialog" aria-hidden="true" style="display: none;">
        <div class="modal-dialog modal-lg" role="document" style="width: 70%;">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="col-md-4">
                     </div>
                     <div class="col-md-4">
                       <div class="dropdown">
                          <button class="btn btn-default btn-lg btn-block dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">HW<span class="caret"></span></button>
                             <ul id="teacher_dropdown_menu" class="dropdown-menu btn-lg btn-block" aria-labelledby="dropdownMenu1">
                          </ul>
                        </div>
                    </div>  
                </div>
                <div class="modal-body" > 
                    <div class="row">
                        <div class="col-md-12" id="show">
                            <button type="button" id="hw_uploads_count" class="btn btn-info">每日上傳平均</button>
                            <button type="button" id="hw_std_states" class="btn btn-info">繳交狀況</button>
                            <button type="button" id="hw_avg_time" class="btn btn-info">次數區段正確率</button>                                                
                        </div>                    
                    </div>            
                         <div class="row" id="charts_display" style="width: 100%; height: 600px;margin: 0 auto"></div>                                 
                    <span id="new_boutton"></span>                                                                                                           
                </div>              
            </div>
        </div>
    </div>
    <script>
        var course = "";//課程代號
        var ts = 0;//班級總人數
        $.ajax({
            url: "getCourseInfo.asmx/gCI",
            method: "post",
            dataType: "json",
            data: { hw: $('#dropdownMenu1').text() },
            success: function (data)
            {
                //console.log("抓漏 "+ data);
                $(data).each(function (index, CS_CSN)
                {                
                    course = CS_CSN.Course_id;
                    ts = CS_CSN.Course_num;                  
                });
                //console.log("抓資料 : " + course + " " + ts);
            },
            error: function (err)
            {

            }
        });

    </script>
    

    <script>        //每日上傳平均 (有曲線的長條圖)      
        $(document).on('click', '#hw_uploads_count', function () {//＊要防呆 HW 預設問題                 
            if ($('#dropdownMenu1').text() != 'HW') {
                // console.log("進入if");
                $.ajax({
                    url: "statistics_hw.asmx/hw_uploads_count",//每日上傳平均
                    method: "post",
                    dataType: "json",
                    data: { hw: $('#dropdownMenu1').text() },
                    success: function (data) {
                        $("#new_boutton").empty();
                        //console.log("success");
                        var date_array = [];
                        var daily_true = [];
                        var d_t = 0;
                        var daily_false = [];
                        var d_f = 0;
                        var total_tof = 0;
                        var avg = [];
                        var count = 0;
                        var t_count = 0;
                        var f_count = 0;
                        //console.log("回傳" + data);
                        $(data).each(function (index, teacher_date) {
                            date = [];
                            //console.log("index " + index);
                            date.push(teacher_date.date);
                            //console.log("回傳1 " + teacher_date.date);
                            //console.log("回傳2 " + teacher_date.true_std);
                            date_array.push(teacher_date.date);
                            daily_true.push(Math.round(teacher_date.true_std * 10) / 10);
                            d_t += teacher_date.true_std;
                            daily_false.push(Math.round(teacher_date.false_std * 10) / 10);
                            d_f += teacher_date.false_std;
                            total_tof += teacher_date.true_std + teacher_date.false_std;
                            if (teacher_date.true_std == 0) {
                                avg.push((teacher_date.true_std + teacher_date.false_std) / 1);
                                t_count++;//對的總數
                            }
                            else if (teacher_date.false_std == 0) {
                                avg.push((teacher_date.true_std + teacher_date.false_std) / 1);
                                f_count++;//錯的總數
                            }
                            else {
                                avg.push((teacher_date.true_std + teacher_date.false_std) / 2);
                            }
                            count++;

                        });
                        var t_percent = ((Math.round(d_t / count * 100) / 100) / (Math.round(total_tof / count * 100) / 100));
                        var f_percent = ((Math.round(d_f / count * 100) / 100) / (Math.round(total_tof / count * 100) / 100));
                        //console.log("陣列date " + date_array);
                        //console.log("答對 " + daily_true);
                        //console.log("答錯 " + daily_false);
                        //console.log("avg " + avg);
                        //console.log("對 ++ " + d_t + " 錯 ++ " + d_f + " 總 :" + Math.round(total_tof / count * 100) / 100);
                        //console.log("對 count " + (count - t_count) + "錯 count " + (count - f_count) + " 全 " + count);
                        //console.log("t " + Math.round(d_t / count * 100) / 100 + " f " + Math.round(d_f / count * 100) / 100);
                        //console.log("t percent " + Math.round(t_percent * 100) + " f percent " + Math.round(f_percent * 100));
                        Highcharts.chart('charts_display', {
                            title: {
                                text: '每日上傳平均 - ' + $('#dropdownMenu1').text()
                            },
                            subtitle: {
                                text: '課程代號: ' + course + ' 人數: ' + ts + '人'
                            },
                            xAxis: {
                                categories: date_array
                            },
                            labels: {
                                items: [{
                                    html: '總平均次數: ' + Math.round(total_tof / count * 100) / 100,
                                    style: {
                                        left: '70%',
                                        top: '18px',
                                        color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                                    }
                                }]
                            },
                            series: [{
                                type: 'column',
                                name: '答對上傳次數',
                                data: daily_true
                            }, {
                                type: 'column',
                                name: '答錯上傳次數',
                                data: daily_false
                            }, {
                                type: 'spline',
                                name: '平均答題',
                                data: avg,
                                marker: {
                                    lineWidth: 2,
                                    lineColor: Highcharts.getOptions().colors[3],
                                    fillColor: 'white'
                                }
                            }, {
                                type: 'pie',
                                name: '百分比%',
                                data: [{
                                    name: '答對 (' + Math.round(d_t / count * 100) / 100 + ')',
                                    y: Math.round(t_percent * 100),
                                    color: Highcharts.getOptions().colors[0] // 答對平均
                                }, {
                                    name: '答錯 (' + Math.round(d_f / count * 100) / 100 + ')',
                                    y: Math.round(f_percent * 100),
                                    color: Highcharts.getOptions().colors[1] // 答錯平均
                                }],
                                center: [100, 80],
                                size: 100,
                                showInLegend: false,
                                dataLabels: {
                                    enabled: false
                                }
                            }]
                        });

                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            }
            else {
                //console.log("==" + $('#dropdownMenu1').text());
            }

        });

    </script>
    
    <script>        //繳交狀況 圓餅圖

        $(document).on('click', '#hw_std_states', function () {
            var tus = 0;
            var ntus = 0;
            var tut = 0;
            var tuf = 0;
            
            if ($('#dropdownMenu1').text() != 'HW') {
                $.ajax({
                    url: "statistics_hw.asmx/avg_correct",
                    method: "post",
                    dataType: "json",
                    data: { hw: $('#dropdownMenu1').text() ,mode:"0"},                 
                    success: function (data)
                    {
                        $("#new_boutton").empty();
                        //var get = JSON.parse(data);                                     
                        $(data).each(function (index, fb) {
                            //ts = fb.total_std;
                            tus = fb.try_upload_std;
                            ntus = fb.no_try_upload_std;
                            tut = fb.try_upload_true;
                            tuf = fb.try_upload_false;
                           
                        });
                        //console.log("測試資料傳回: 全部學生" + tus + " ntus " + ntus + " tut " + tut + " tuf " + tuf);
                        //這邊是API原生不用看
                        var colors = Highcharts.getOptions().colors,
                            categories = ['嘗試繳交', '未嘗試繳交'],
                            data = [{
                                y: tus,
                                color: colors[2],
                                drilldown: {
                                    name: '1',
                                    categories: ['答對','未答對'],
                                    data: [tut, tuf],
                                    color: colors[2]
                                }
                            }, {
                                y: ntus,
                                color: colors[5],
                                drilldown: {
                                    name: '2',
                                    categories: ['缺交'],
                                    data: [ntus],
                                    color: colors[5]
                                }
                            }],
                            browserData = [],
                            versionsData = [],
                            i,
                            j,
                            dataLen = data.length,
                            drillDataLen,
                            brightness;


                        // Build the data arrays
                        for (i = 0; i < dataLen; i += 1) {

                            // add browser data
                            browserData.push({
                                name: categories[i],
                                y: data[i].y,
                                color: data[i].color
                            });

                            // add version data
                            drillDataLen = data[i].drilldown.data.length;
                            for (j = 0; j < drillDataLen; j += 1) {
                                brightness = 0.2 - (j / drillDataLen) / 5;
                                versionsData.push({
                                    name: data[i].drilldown.categories[j],
                                    y: data[i].drilldown.data[j],
                                    color: Highcharts.Color(data[i].color).brighten(brightness).get()
                                });
                            }
                        }

                        // Create the chart
                        Highcharts.chart('charts_display', {
                            chart: {
                                type: 'pie'
                            },
                            title: {
                                text: '繳交狀況 - '+ $('#dropdownMenu1').text()
                            },
                            subtitle: {
                                text: '課程代號: ' + course + ' 人數: ' + ts+'人'//這要改用asmx回傳
                            },
                            yAxis: {
                                title: {
                                    text: 'Total percent market share'
                                }
                            },
                            plotOptions: {
                                pie: {
                                    shadow: false,
                                    center: ['50%', '50%']
                                }
                            },
                            tooltip: {
                                valueSuffix: '人'
                            },
                            series: [{
                                name: '個數',
                                data: browserData,
                                size: '60%',
                                dataLabels: {
                                    formatter: function () {
                                        return this.y > 5 ? this.point.name : null;
                                    },
                                    color: '#ffffff',
                                    distance: -30
                                }
                            }, {
                                name: '個數',
                                data: versionsData,
                                size: '80%',
                                innerSize: '60%',
                                dataLabels: {
                                    formatter: function () {
                                        // display only if larger than 1
                                        return this.y > 1 ? '<b>' + this.point.name + ':</b> ' + this.y + '人' : null;
                                    }
                                }
                            }]
                            ///end不用看
                        });
                    },
                    error: function (err) {



                    }
                    
                });

            }    
            else
            {

            }    
      });
        
    </script>
    

    <script>        //次數區段正確率 圖(彩色長條圖) 累計
        var chart = null;
        $(document).on('click', '#hw_avg_time,#not_Accumulate', function () {
            $.ajax({
                url: "statistics_hw.asmx/avg_correct",
                method: "post",
                dataType: "json",
                data: { hw: $('#dropdownMenu1').text(), mode: "1" },
                success: function (data)
                {
                    $("#new_boutton").empty();
                    var x = [];
                    var y = [];
                    var t = 0;
                    var avg = [];
                    
                    //console.log("max " + data.max);
                    var str = '<button type="button" class="btn btn-danger" id="plain">直列顯示</button>' +
                              '<button type="button" class="btn btn-danger" id="inverted">橫列顯示</button>'+
                              '<button type="button" class="btn btn-success" id="Accumulate">不累計顯示</button>';
                    $("#new_boutton").append(str);
                    $(data).each(function (index, feedback_data)
                    {
                        //console.log("index " + index + " US: " + feedback_data.upload_session + " avg: " + Math.round(feedback_data.avg*100)/100);
                        t += Math.ceil(feedback_data.avg * 100) / 100;
                        x.push(feedback_data.upload_session);
                        y.push(Math.ceil(t * 100) / 100);
                        avg.push(Math.ceil(feedback_data.avg * 100) / 100);
                    });

                    //console.log("avg回傳: " + avg + " Y: " + y);
                    chart = Highcharts.chart('charts_display', {
                        
                        title: {
                            text: '次數區段正確率(累計) - '+$('#dropdownMenu1').text()
                        },

                        subtitle: {
                            text: '課程代號: ' + course + ' 人數: ' + ts + '人'
                        },

                        xAxis: {
                            categories: x
                        },

                        series: [{
                            type: 'column',
                            colorByPoint: true,
                            data: y,
                            showInLegend: false
                        }]

                    });
                   
                },
                error: function (err)
                {
                    console.log("err");
                }
            });
        });
    </script>

    <script>        //次數區段正確率 圖(彩色長條圖)不累計
        var chart = null;
        $(document).on('click', '#Accumulate', function () {
            $.ajax({
                url: "statistics_hw.asmx/avg_correct",
                method: "post",
                dataType: "json",
                data: { hw: $('#dropdownMenu1').text(), mode: "1" },
                success: function (data) {
                    $("#new_boutton").empty();
                    var x = [];
                    var y = [];//累計
                    var t = 0;
                    var avg = [];//不累計


                    var str = '<button type="button" class="btn btn-danger" id="plain">直列顯示</button>' +
                              '<button type="button" class="btn btn-danger" id="inverted">橫列顯示</button>' +
                              '<button type="button" class="btn btn-success" id="not_Accumulate">累計顯示</button>';

                    $("#new_boutton").append(str);


                    //console.log("max " + data.max);                  
                    $(data).each(function (index, feedback_data) {
                        //console.log("index " + index + " US: " + feedback_data.upload_session + " avg: " + Math.round(feedback_data.avg*100)/100);
                        t += Math.ceil(feedback_data.avg * 100) / 100;
                        x.push(feedback_data.upload_session);
                        y.push(Math.ceil(t * 100) / 100);
                        avg.push(Math.ceil(feedback_data.avg * 100) / 100);
                    });

                    //console.log("avg回傳: " + avg + " Y: " + y);
                    chart = Highcharts.chart('charts_display', {

                        title: {
                            text: '次數區段正確率(不累計) - ' + $('#dropdownMenu1').text()
                        },

                        subtitle: {
                            text: '課程代號: ' + course + ' 人數: ' + ts + '人'
                        },

                        xAxis: {
                            categories: x
                        },

                        series: [{
                            type: 'column',
                            colorByPoint: true,
                            data: avg,//不累計資料
                            showInLegend: false
                        }]

                    });

                },
                error: function (err) {
                    console.log("err");
                }
            });
        });
    </script>


    <script>
        $(document).on('click', '#plain', function () {
            chart.update({
                chart: {
                    inverted: false,
                    polar: false
                },               
            });
        });
        $(document).on('click', '#inverted', function () {            
            chart.update({
                chart: {
                    inverted: true,
                    polar: false
                },             
            });
        });     
    </script>
    
     <script>
             //$("#teacher_dropdown_menu").html(str);//塞進去Dropdownmenu
             $(document).on('click', '#teacher_dropdown_menu li a', function () {//等到網頁 loading 後觸發 click 之後才會去找元件 [on('click')這邊是塞事件]然後動態抓
                 $("#dropdownMenu1").text($(this).text());
                 $(charts_display).empty();
                 $(new_boutton).empty();
             });
                </script>
                <script>
                    $.ajax({
                        url: "teacher_show_hwstatus.asmx/show_status",
                        method: "post",
                        dataType: "json",
                        success: function (data)//form teacher_show_hwstatus.asmx
                        {
                            $("#teacher_dropdown_menu").empty();
                            $(data).each(function (index, hw_list) {
                                var str = "";//這邊改成動態抓資料庫值
                                str += '<li><a>' + hw_list.hw_name + '</a></li>'; //'<li><a action=2>aa' + i + '</a></li>'; 加action可以利用分類使用switch($(this).attr("action"))控制她要跑啥function
                                $("#teacher_dropdown_menu").append(str);
                            });//初始化

                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });

        </script>
        <script> //老師查看新增作業JQ
            $(document).ready(function () {
                $("#show_hwstatus").click(function () {
                    $.ajax({
                        url: "teacher_show_hwstatus.asmx/show_status",
                        method: "post",
                        dataType: "json",
                        success: function (data)//form teacher_show_hwstatus.asmx
                        {
                            $("#std_hwstate_table tbody").empty();
                            var list = "";
                            $(data).each(function (index, teacher) {
                                list = "";
                                list += "<tr><td>" + teacher.hw_name + "</td>" +
                                        "<td>" + teacher.hw_answer_script + "</td>" +
                                        "<td>" + teacher.hw_input_script + "</td>" +
                                         "<td>" + teacher.hw_mode + "</td>" +
                                         "<td>" + teacher.hw_structure_condtion + "</td></tr>";
                                $("#std_hwstate_table tbody").append(list);
                                //console.log("index " + index);
                            });
                        },
                        error: function (err) {
                            console.log(err);
                        }
                    });
                });
            });

    </script>
    <script>
        $(document).ready(function () {//這個事先在網頁loading前檢查元件在綁定下面元件   [開始就找]
            $("#charts").click(function () {
                    

            });
        });
    </script>


    <div class="row">
        <div class="col-md-2"></div>
        <div class="col-md-8" style="padding: 1%;"> <%--padding: 1%;--%>
            <div class="panel panel-primary">
                <div class="panel-heading">設定作業</div>
                <div class="panel-body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="input-group" style="margin-left: 1%;margin-bottom:1.5%;">
                                <span class="input-group-addon" id="basic-addon1">作業名稱</span>
                                <input type="text" name="textbox_hwname" id="textbox_hwname" runat="server" class="form-control" placeholder="請輸入作業名稱..." aria-describedby="basic-addon1" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="input-group" style="margin-left: 1%;margin-bottom:1.5%;">
                                <span class="input-group-addon" id="basic-addon12">答案Cpp</span>
                                <asp:FileUpload ID="FileUpload1" class="form-control" runat="server" />
                            </div>
                        </div>
                        <%--style="border:2px solid #eee" //可以描邊框調整--%>
                    </div>

                    <div class="row">
                        <div class="col-md-12">
                            <div class="input-group " style="margin-left: 1%;margin-bottom:1.5%;">                             
                                    <span class="input-group-addon" id="basic-addon13">結構設定</span>
                                    <div class="container form-control">
                                        <asp:Label ID="Label1" Style="margin-left: 1%;" runat="server" Text=" For:"></asp:Label>
                                        <asp:TextBox ID="TextBox_for" runat="server" Width="5%"></asp:TextBox>
                                        <asp:Label ID="Label2" runat="server" Text=" Do_While:"></asp:Label>
                                        <asp:TextBox ID="TextBox_DoWhile" runat="server" Width="5%"></asp:TextBox>
                                        <asp:Label ID="Label3" runat="server" Text=" While:"></asp:Label>
                                        <asp:TextBox ID="TextBox_While" runat="server" Width="5%"></asp:TextBox>
                                        <asp:Label ID="Label4" runat="server" Text=" If:"></asp:Label>
                                        <asp:TextBox ID="TextBox_If" runat="server" Width="5%"></asp:TextBox>
                                        <asp:Label ID="Label5" runat="server" Text=" switch:"></asp:Label>
                                        <asp:TextBox ID="TextBox_switch" runat="server" Width="5%"></asp:TextBox>
                                    </div>
                                </div>
                        </div>
                    </div>


                    <div class="row">
                        <div class="col-md-12">
                            <div class="input-group" style="margin-left: 1%;margin-bottom:1.5%;">
                                    <span class="input-group-addon" id="basic-addon14">測資類型</span>
                                    <asp:DropDownList ID="DropDownList_Chose_TestType" CssClass="form-control" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList_Chose_TestType_SelectedIndexChanged">
                                        <asp:ListItem Value="0">請選擇模式</asp:ListItem>
                                        <asp:ListItem Value="1">自訂輸入</asp:ListItem>
                                        <asp:ListItem Value="2">不跑測資</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                        </div>
                    </div>

              <div class="row">
                        <div class="col-md-12">
                            <div class="input-group" style="margin-left: 1%;margin-bottom:1.5%;">
                                <span class="input-group-addon" id="basic-addon6">自訂輸入</span>
                                <div class="container form-control">
                                    <asp:TextBox ID="TextBox_Customize_Input" runat="server" OnTextChanged="TextBox_Customize_Input_TextChanged" Visible="False" Width="153px"></asp:TextBox>
                                    <asp:Label ID="Label_inputWarning" runat="server" ForeColor="Red" Text="(請輸入值)" Visible="False"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-12">
                            <asp:Button ID="send_button" Style="margin-left: 13.5%;" runat="server" OnClick="send_Button_Click" Text="設定完成" CssClass="btn btn-lg btn-primary" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-2"></div>
    </div>

    <div class="modal fade" id="check_hwlist" tabindex="-1" role="dialog" aria-hidden="true" style="display: none;">
        <div class="modal-dialog" role="document" style="width: 100%;">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="headertekst">已新增作業</h2>
                </div>
                <div class="modal-body">
                    <table id="std_hwstate_table" class="table">
                        <thead>
                            <tr>
                                <th style="width: 5%;">作業名稱</th>
                                <th style="width: 20%;">答案腳本</th>
                                <th style="width: 20%;">輸入腳本</th>
                                <th style="width: 5%;">批改模式</th>
                                <th style="width: 9%;">設定結構</th>
                            </tr>
                        </thead>
                        <tbody>
                        </tbody>
                    </table>

                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Announce_from" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">發布公告</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="form-group">
                        <label for="recipient-name" class="form-control-label">公告標題:</label>
                        <input type="text" runat="server" class="form-control" id="annouce_title"></input>
                    </div>
                    <div class="form-group">
                        <label for="message-text" class="form-control-label">訊息:</label>
                        <textarea runat="server" class="form-control" id="annouce_message"></textarea>
                    </div>

                </div>
                <div class="modal-footer">
                    <button type="button" runat="server" id="button_send_message" class="btn btn-primary" onserverclick="button_send_message_Click">發布</button>
                </div>
            </div>
        </div>
    </div>
    <br />
    <div class="row">
        <div class="col-md-2">
        </div>
        <div class="col-md-4">
            <div class="panel panel-primary">
                <div class="panel-heading">程式測資輸出</div>
                <div class="panel-body">
                    <asp:TextBox ID="TextBox2" runat="server" Height="365px" TextMode="MultiLine" Width="100%" ReadOnly="True"></asp:TextBox>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            <div class="panel panel-primary">
                <div class="panel-heading">答案輸出</div>
                <div class="panel-body">
                    <asp:TextBox ID="TextBox3" runat="server" Height="365px" TextMode="MultiLine" Width="100%" ReadOnly="True"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-md-2">
        </div>
        <div class="col-md-10">


            <asp:GridView ID="GridView_showHW" runat="server" BackColor="#DEBA84" BorderColor="#DEBA84" BorderStyle="None" BorderWidth="1px" CellPadding="3" CellSpacing="2">
                <FooterStyle BackColor="#F7DFB5" ForeColor="#8C4510" />
                <HeaderStyle BackColor="#A55129" Font-Bold="True" ForeColor="White" />
                <PagerStyle ForeColor="#8C4510" HorizontalAlign="Center" />
                <RowStyle BackColor="#FFF7E7" ForeColor="#8C4510" />
                <SelectedRowStyle BackColor="#738A9C" Font-Bold="True" ForeColor="White" />
                <SortedAscendingCellStyle BackColor="#FFF1D4" />
                <SortedAscendingHeaderStyle BackColor="#B95C30" />
                <SortedDescendingCellStyle BackColor="#F1E5CE" />
                <SortedDescendingHeaderStyle BackColor="#93451F" />
            </asp:GridView>

        </div>
    </div>



    <asp:Panel ID="Panel1" runat="server" Height="20px" Width="350px" HorizontalAlign="Right" Direction="LeftToRight">
        <asp:Table ID="Table1" runat="server">
        </asp:Table>
    </asp:Panel>
</asp:Content>
