using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fougerite;
using System.IO;

namespace JuliUpdater
{
    /// <summary>
    /// This plugin is responsible for updating the files on your server (only .dll)
    /// 1- Add the files you want to update to the "Updates" folder
    /// 2- Remove the extension .dll from these files that you put in "Updates"
    /// 3- Use the command / update all
    /// 
    /// Then the plugin will take care of automatically searching the files that need updating
    /// Also the plugin will keep a copy of the files that were updated
    /// </summary>
    
    public class JuliUpdaterClass : Fougerite.Module
    {
        public override string Name { get { return "JuliUpdater"; } }
        public override string Author { get { return "Salva/Juli"; } }
        public override string Description { get { return "JuliUpdater"; } }
        public override Version Version { get { return new Version("1.0"); } }

        public string red = "[color #B40404]";
        public string blue = "[color #81F7F3]";
        public string green = "[color #82FA58]";
        public string yellow = "[color #F4FA58]";
        public string orange = "[color #FF8000]";
        public string pink = "[color #FA58F4]";
        public string white = "[color #FFFFFF]";

        public static System.IO.StreamWriter file;

        public string RootDir = Util.GetRootFolder();

        public string UpdatesFolder = "";

        public override void Initialize()
        {
            if (!Directory.Exists(ModuleFolder + "\\Updates\\"))
            {
                Directory.CreateDirectory(ModuleFolder + "\\Updates\\");
            }

            UpdatesFolder = ModuleFolder + "\\Updates\\";

            Hooks.OnCommand += OnCommand;
        }
        public override void DeInitialize()
        {
            Hooks.OnCommand -= OnCommand;
        }
        public void OnCommand(Fougerite.Player player, string cmd, string[] args)
        {
            if (!player.Admin) { return; }
            if (cmd == "update")
            {
                if (args.Length == 0)
                {
                    player.MessageFrom(Name, "/update " + orange + " List of commands");
                    player.MessageFrom(Name, "/update all" + orange + " Update all files into Update folder");
                }
                else
                {
                    if (args[0] == "all")
                    {
                        UpdateAll(player);
                    }
                }
            }
        }

        public void UpdateAll(Fougerite.Player pl)
        {
            DirectoryInfo diroot = new DirectoryInfo(RootDir);
            DirectoryInfo diupdates = new DirectoryInfo(UpdatesFolder);

            foreach (var ffile in diupdates.GetFiles())
            {
                string FileOrigenName = ffile.Name;
                string FileDestinyRoute = "";

                //comprueba que existe el archivo que se quiera actualizar en la carpeta UPDATES
                if (File.Exists(UpdatesFolder + FileOrigenName))
                {
                    pl.MessageFrom(Name, "ORIGEN: " + FileOrigenName + " is ready to update OK!");
                }
                else
                {
                    Logger.Log("ERROR ------>>>>> ORIGEN: " + FileOrigenName + " NOT FOUND!");
                    continue;
                }

                //busca si el servidor contiene ese archivo y guarda su ruta como destino
                foreach (var fi in diroot.GetFiles(FileOrigenName + ".dll", SearchOption.AllDirectories))
                {
                    if (fi.Directory.Name.Contains("Prepatched_AssemblyDLL"))
                    {
                        pl.MessageFrom(Name, "Prepatched_AssemblyDLL FOLDER?????? comprobar");
                        continue;
                    }

                    pl.MessageFrom(Name, "Filename: " + fi.Name + " ### Route: " + fi.DirectoryName);
                    Logger.Log("Filename: " + fi.Name + " ### Route: " + fi.DirectoryName);
                    FileDestinyRoute = fi.DirectoryName + "\\";
                    break;
                }

                //comprueba que existe el mismo archivo en la carpeta de destino pero con .dll
                if (File.Exists(FileDestinyRoute + FileOrigenName + ".dll"))
                {
                    pl.MessageFrom(Name, "DESTINY: " + FileOrigenName + ".dll " + " is ready to be updated OK!");
                }
                else
                {
                    Logger.Log("ERROR ------>>>>> DESTINY: " + FileDestinyRoute + FileOrigenName + ".dll " + " NOT FOUND!");
                    continue;
                }

                //arrojar un mensaje que todo esta bien con este archivo

                //renombrar el archivo de destino y quitarle el dll (quizas añadir la fecha en el nombre nuevo)
                try
                {
                    File.Move(FileDestinyRoute + FileOrigenName + ".dll", FileDestinyRoute + FileOrigenName + " h." + System.DateTime.Now.Hour + " m." + System.DateTime.Now.Minute + " s." + System.DateTime.Now.Second);
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR ------>>>>> RENAMING: " + FileDestinyRoute + FileOrigenName + ".dll " + " --TO-- " + FileDestinyRoute + FileOrigenName + System.DateTime.Now.ToString() + " <<<<<<<<<<<<<< " + ex.ToString());
                    continue;
                }

                //entonces copiar desde updates hacia el destino e incluir .dll
                try
                {
                    File.Move(UpdatesFolder + FileOrigenName, FileDestinyRoute + FileOrigenName + ".dll");
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR ------>>>>> MOVING: " + UpdatesFolder + FileOrigenName + " --TO-- " + FileDestinyRoute + FileOrigenName + ".dll <<<<<<<<<<<<<< " + ex.ToString());
                    continue;
                }
                pl.MessageFrom("UPDATER", green + "OK! " + white + FileOrigenName + ".dll");
            }
        }
    }
}
