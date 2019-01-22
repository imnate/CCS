<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="OSMVA_UploadHW.Test_send" MasterPageFile="~/MainFrame.Master" %>


<asp:Content ID="MainContent" ContentPlaceHolderID="mainContentPlaceHolder" runat="server">

<%--    <script>
        function abc() {
            alert("hello world");

        }
    </script>--%>


    

    <br />
    <br />
    <br />
    <br />
    <br />
    <div runat ="server" id ="sss"> 

    </div>

    <p id="abcdef"></p>

    <div class="row">
        <div class="col-md-4">
            
        </div>
        <div class="col-md-4">
            <div class="panel panel-danger">
                <div class="panel-heading"><span class="glyphicon glyphicon-bookmark"></span>最新消息</div>
                <div class="panel-body">
                    <asp:ListBox ID="ListBox_newMessage" Style ="width: 100%; height: 150px;" runat="server"></asp:ListBox>
                </div>
            </div>
        </div>
        <div class="col-md-4">

        </div>
    </div>


    <div class="row">
        <div class="col-md-4">
            
        </div>
        <div class="col-md-4">
            <div class="panel panel-info">
                <div class="panel-heading"><span class="glyphicon glyphicon-exclamation-sign"></span>登入帳號</div>
                <div class="panel-body">
                    <div class="row" style="margin-bottom: 20px;">
                        <div class="form-group">
                            <asp:Label ID="Label1" runat="server" Text="帳號:" CssClass="control-label col-md-2" Font-Size="Large"></asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox ID="TextBox1" runat="server" CssClass="control-label form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group">
                            <asp:Label ID="Label2" runat="server" Text="密碼:" CssClass="control-label col-md-2" Font-Size="Large"></asp:Label>
                            <div class="col-md-5">
                                <asp:TextBox ID="TextBox2"  runat="server" CssClass="control-label form-control" TextMode="Password"></asp:TextBox>
                            </div>
                            <div class="col-md-2">
                                <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="登入" class="btn btn-success" Font-Size="Medium" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-4">
            
        </div>
    </div>


<%--    <script>
        abc();
    </script>
    <script>
        $("#abcdef").html("helloworld");
    </script>--%>

</asp:Content>
