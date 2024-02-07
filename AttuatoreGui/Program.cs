// See https://aka.ms/new-console-template for more information

using AttuatoreMqtt;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;


/*
 * Questa e' la Gui usata per inserire in Input i dati all'attuatore.
 * La Gui inizializza l'attuatore e poi attraverso i suoi metodi gli passa i dati come l'id, azienda e campo.
 */

var attuatore = new Attuatore();

Application.Init();

var view = new Window("Attuatore Gui");

#region Azienda
var labelAzienda = new Label("Azienda: ");

var textAzienda = new TextField()
{
    X = 10,
    Y = 0,
    Width = Dim.Fill(),
    Height = Dim.Percent(10)
};


textAzienda.TextChanged += async text =>
{
    attuatore.Azienda = (string)textAzienda.Text;
};

view.Add(labelAzienda, textAzienda);
#endregion

#region Campo
var labelCampo = new Label("Campo: ") { Y = 2};

var textCampo = new TextField()
{
    X = 10,
    Y = 2,
    Width = Dim.Fill(),
    Height = Dim.Percent(10)
};

textCampo.TextChanged += async text =>
{
    attuatore.Campo = (string)textCampo.Text;
};

view.Add(labelCampo, textCampo);
#endregion

#region Id
var labelId = new Label("Id: ") { Y = 4 };

var textId = new TextField()
{
    X = 10,
    Y = 4,
    Width = Dim.Fill(),
    Height = Dim.Percent(10)
};

textId.TextChanged += async text =>
{
    attuatore.Id = (string)textId.Text;
    await attuatore.Disconnect();
    attuatore.Start();
};

view.Add(labelId, textId);
#endregion

#region Led
var state = new Label(attuatore.State) { X = Pos.Center() ,Y = Pos.Center()};
var colors = new ColorScheme();
colors.Normal = new Attribute(Color.White, Color.Red);
state.ColorScheme = colors;
view.Add(state);
#endregion

Application.Top.Add(view);

Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), delegate (MainLoop loop)
{
    switch (attuatore.State)
    {
        case "On":
            colors.Normal = new Attribute(Color.White, Color.Green);
            break;
        case "Off":
            colors.Normal = new Attribute(Color.White, Color.Red);
            break;
    }
    state.Text = attuatore.State;
    return true;
});


Application.Run();
Application.Shutdown();