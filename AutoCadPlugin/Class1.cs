using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;


namespace AutoCadTest
{
    public class Commands
    {

        [CommandMethod("TEST")]
        public void Test()
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            var db = doc.Database;
            var ed = doc.Editor;
            using (var tr = db.TransactionManager.StartTransaction()) {
                tr.Commit();
            }
        }



        [CommandMethod("EOFTEST")]
        static public void ExtractObjectsFromFile()

        {
            Document doc =

              Application.DocumentManager.MdiActiveDocument;

            Editor ed = doc.Editor;


            // Ask the user to select a file

            PromptResult res =

              ed.GetString(

                "\nEnter the path of a DWG or DXF file: "

              );


            if (res.Status == PromptStatus.OK) {

                // Create a database and try to load the file

                Database db = new Database(false, true);

                using (db) {

                    try {

                        db.ReadDwgFile(

                          res.StringResult,

                          System.IO.FileShare.Read,

                          false,

                          ""

                        );

                    }

                    catch (System.Exception) {

                        ed.WriteMessage(

                          "\nUnable to read drawing file."

                        );

                        return;

                    }


                    Transaction tr =

                      db.TransactionManager.StartTransaction();

                    using (tr) {

                        // Open the blocktable, get the modelspace

                        BlockTable bt =

                          (BlockTable)tr.GetObject(

                            db.BlockTableId,

                            OpenMode.ForRead

                          );


                        BlockTableRecord btr =

                          (BlockTableRecord)tr.GetObject(

                            bt[BlockTableRecord.ModelSpace],

                            OpenMode.ForRead

                          );



                        // Iterate through it, dumping objects

                        foreach (ObjectId objId in btr) {

                            Entity ent =

                              (Entity)tr.GetObject(

                                objId,

                                OpenMode.ForRead

                              );


                            // Let's get rid of the standard namespace

                            const string prefix =

                              "Autodesk.AutoCAD.DatabaseServices.";

                            string typeString =

                              ent.GetType().ToString();

                            if (typeString.Contains(prefix))

                                typeString =

                                  typeString.Substring(prefix.Length);


                            ed.WriteMessage(

                              "\nEntity " +

                              ent.ObjectId.ToString() +

                              " of type " +

                              typeString +

                              " found on layer " +

                              ent.Layer +

                              " with colour " +

                              ent.Color.ToString()

                            );

                        }

                    }

                }

            }

        }

    }

}