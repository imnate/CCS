<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CppJudge.aspx.cs" Inherits="OSMVA_UploadHW.upload" MasterPageFile="~/MainFrame.Master" ValidateRequest="false" %>


<asp:Content ID="MainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.2.0/jquery.min.js"></script>
    <link rel="stylesheet" href="bootstrap-3.3.7-dist/css/menu-scroll.css" />
    <script>
        $(document).ready(function () {//必要1
            $("#search").click(function () {//必要2
                $.ajax({////必要3  打開ajax
                    url: "table_refresh.asmx/getHwTable", //給他連結
                    method: "post", //設定方法 post會回傳給伺服                   
                    dataType: "json",                                    
                    success: function (data) //上面url get 成功 接下去做的function 
                    {
                        $("#HWstate_table tbody").empty();//清#idtable 的所有tbody                   
                        var str = "";
                        $(data).each(function (index, std) {
                            console.log("index: " + index);
                            str = ""; //這不加進去會有資料重複讀取所以要清空                          
                            str += "<tr><td>" + std.hw_name + "</td>" +
                                   "<td>" + std.hw_update + "</td>" +
                                   "<td>" + std.hw_times + "</td>" +
                                   "<td>" + std.hw_stuts + "</td>" +
                                   "<td>" + std.hw_encode + "</td></tr>";
                            console.log(str);
                        $("#HWstate_table tbody").append(str);//把上面被絕後的孫子 再加進去 tbody變成他的child
                        });
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            }
        );
        });
    </script>
    <nav class="navbar navbar-default navbar-fixed-top">
        <div class="container-fluid">
            <div class="navbar-header">
                <a class="navbar-brand" href="#">線上程式批改系統</a>
            </div>
            <ul class="nav navbar-nav">
                <li class="nav-item">                               
                    <a id="search" class="nav-link dropdown-toggle" href="#" data-toggle="modal" data-target="#check_hwlist" role="button" aria-haspopup="true" aria-expanded="false"><span class="glyphicon glyphicon-search"></span>查詢</a>                           
                </li>
            </ul>          
        </div>
    </nav>
    <div class="modal fade" id="check_hwlist" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel2" aria-hidden="true">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h2 class="headertekst">作業繳交狀況</h2>   
                     </div>
                    <div class="modal-body">                        
                            <table id="HWstate_table" class="table">
                                <thead>
                                    <tr>
                                        <th>作業名稱</th>
                                        <th>繳交日期</th>
                                        <th>繳交次數</th>
                                        <th>答題狀態</th>
                                        <th>檔名驗證碼</th>
                                    </tr>
                                </thead>
                                <tbody>                                                                  
                                </tbody>
                            </table>
                    </div>
                </div>
            </div>
        </div>
    <br />
    <br />
    <br />
    <div class="row">
        <div class="col-md-4">
        </div>
        <div class="col-md-4">
        </div>
    </div>

    <div class="row">
        <div class="col-md-4">
            <div class="panel panel-primary">
                <div class="panel-heading">上傳作業</div>
                <div class="panel-body">
                    <asp:DropDownList ID="DropDownList_HW" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged" Width="314px">
                        <asp:ListItem>請選擇上傳作業</asp:ListItem>
                    </asp:DropDownList>
                    <asp:FileUpload ID="FileUpload" runat="server" />
                    <asp:TextBox ID="TextBox_Code" runat="server" TextMode="MultiLine" ReadOnly="True" Style="width: 100%; height: 325px;"></asp:TextBox>
                    <asp:Button ID="compiler" runat="server" OnClick="compiler_Click" Text="繳交 &amp; Compiler" CssClass="btn btn-success" />
                    <asp:Button ID="Button_Reupdate" runat="server" Text="重新上傳" OnClick="Button2_Click" CssClass="btn btn-success" />
                </div>
            </div>

        </div>
        <div class="col-md-4">

            <div class="panel panel-primary">
                <div class="panel-heading">學生輸出</div>
                <div class="panel-body">

                    <asp:TextBox ID="TextBox2" runat="server" ReadOnly="True" Style="width: 100%; height: 403px; margin-right: 0px; margin-top: 0px" TextMode="MultiLine" Font-Size="Large"></asp:TextBox>

                    <asp:Button ID="Button_excute" runat="server" OnClick="Button3_Click" Text="比對" CssClass="btn btn-success" Visible="False" />

                </div>
            </div>

        </div>

        <div class="col-md-4">

            <div class="panel panel-primary">
                <div class="panel-heading">老師輸出</div>
                <div class="panel-body">

                    <asp:TextBox ID="TextBox3" runat="server" ReadOnly="True" Style="width: 100%; height: 403px; margin-right: 0px; margin-top: 0px" TextMode="MultiLine" Font-Size="Large"></asp:TextBox>

                </div>
            </div>

        </div>
    </div>


    <div class="row">
        <div class="col-md-4">

            <div class="panel panel-info">
                <div class="panel-heading">系統回饋</div>
                <div class="panel-body">
                    <asp:ListBox ID="ListBox_system_feedback" runat="server" Style="width: 100%; height: 175px; margin-right: 0px; margin-top: 0px" Font-Size="Small"></asp:ListBox>

                </div>
            </div>

        </div>
        <div class="col-md-8">

            <div class="panel panel-success">
                <div class="panel-heading">圖片輸出</div>
                <div class="panel-body">

                    <asp:Image ID="Image_std_pic" Height="2000px" Width="1000" class="img-thumbnail" runat="server" /><!--ImageUrl=""-->

                </div>
            </div>

        </div>
    </div>

    <%--助解--%>
</asp:Content>
