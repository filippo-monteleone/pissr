// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using SensoreMqtt;
using Terminal.Gui;


// Questa e' la classe che si occupa di emulare il sensore attraverso la Gui

Application.Init();

var win = new Window("Sensore")
{
    X = 0,
    Y = 1,
    Width = Dim.Percent(70),
    Height = Dim.Fill() - 1,
};

var info = new Window("Info")
{
    X = Pos.Right(win) ,
    Y = 1,
    Width = Dim.Percent(30),
    Height = Dim.Fill() - 1,
};

var sensore = new Sensore();
sensore.Start();

#region Azienda
var AziendaLabel = new Label("Azienda: ");
var AziendaField = new TextField() { X = 10, Width = Dim.Fill() };

AziendaField.TextChanged += text =>
{
    sensore.Azienda = AziendaField.Text.ToString();
};

info.Add(AziendaLabel, AziendaField);
#endregion

#region Campo
var CampoLabel = new Label("Campo: ") { Y = 2 };
var CampoField = new TextField() { X = 10, Y = 2, Width = Dim.Fill() };

CampoField.TextChanged += text =>
{
    sensore.Campo = CampoField.Text.ToString();
};

info.Add(CampoLabel, CampoField);
#endregion

#region Id
var IdLabel = new Label("Id: ") { Y = 4 };
var IdField = new TextField() { X = 10, Y = 4, Width = Dim.Fill() };

IdField.TextChanged += text =>
{
    sensore.Id = IdField.Text.ToString();
};

info.Add(IdLabel, IdField);
#endregion


#region Temperatura
var TempLabel = new Label("Temperatura:") { X = 0, Y = 0 };
var TempField = new TextField() { X = 25, Y = 0, Width = Dim.Percent(20)};
var TempButton = new Button("Invia") {X = Pos.Right(TempField) + 5, Y = 0};

TempButton.Clicked += async () =>
{
    try
    {
        var tempValue = double.Parse(TempField.Text.ToString() ?? "0");
        var id = int.Parse(IdField.Text.ToString() ?? "0");
        await sensore.SendData((0, tempValue, id));
    } catch { }
};

win.Add(TempLabel, TempField, TempButton);
#endregion

#region Radiazione
var RadLabel = new Label("Radiazione solare:") { X = 0, Y = 2 };
var RadField = new TextField() { X = 25, Y = 2, Width = Dim.Percent(20) };
var RadButton = new Button("Invia") { X = Pos.Right(RadField) + 5, Y = 2 };

RadButton.Clicked += async () =>
{
    try
    {

        var radValue = double.Parse(RadField.Text.ToString() ?? "0");
        var id = int.Parse(IdField.Text.ToString() ?? "0");
        await sensore.SendData((1, radValue, id));
    } catch {}
};

win.Add(RadLabel, RadField, RadButton);
#endregion

#region Umidita
var HumLabel = new Label("Umidita:") { X = 0, Y = 4 };
var HumField = new TextField() { X = 25, Y = 4, Width = Dim.Percent(20) };
var HumButton = new Button("Invia") { X = Pos.Right(HumField) + 5, Y = HumLabel.Y };

HumButton.Clicked += async () =>
{
    try
    {

        var humValue = double.Parse(HumField.Text.ToString() ?? "0");
        var id = int.Parse(IdField.Text.ToString() ?? "0");
        await sensore.SendData((2, humValue, id));
    }
    catch
    {

    }
};

win.Add(HumLabel, HumField, HumButton);
#endregion

#region Pioggia
var RainLabel = new Label("Pioggia:") { X = 0, Y = 6 };
var RainField = new TextField() { X = 25, Y = 6, Width = Dim.Percent(20) };
var RainButton = new Button("Invia") { X = Pos.Right(RainField) + 5, Y = RainLabel.Y };

RainButton.Clicked += async () =>
{
    try
    {
        var rainValue = double.Parse(RainField.Text.ToString() ?? "0");
        var id = int.Parse(IdField.Text.ToString() ?? "0");
        await sensore.SendData((3, rainValue, id));
    }
    catch {    }
};

win.Add(RainLabel, RainField, RainButton);
#endregion

Application.Top.Add(win, info);
Application.Run();
Application.Shutdown();