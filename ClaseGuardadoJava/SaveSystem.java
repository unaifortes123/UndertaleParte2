import java.io.File;
import java.io.FileWriter;

public class SaveSystem
{
    public static void main(String[] args)
    {
        String json;
        String base;
        File folder;
        File saveFile;
        FileWriter writer;

        if (args.length == 0)
        {
            System.out.printf("No JSON received");
            return;
        }

        json = args[0];

        try
        {
            base = System.getProperty("user.home");

            File jarFile = new File(
                SaveSystem.class
                    .getProtectionDomain()
                    .getCodeSource()
                    .getLocation()
                    .toURI()
            );

            folder = new File(jarFile.getParentFile().getParentFile(), "Save");

            if (!folder.exists()) // Esto verifica si la carpeta esta o no
            {
                folder.mkdirs(); // Crea la carpeta si no existe
            }

            saveFile = new File(folder, "save.json"); // Crea el archivo "save.json" dentro de la carpeta "Save"

            writer = new FileWriter(saveFile); // Crea un FileWriter para escribir en el archivo "save.json"
            writer.write(json); // Escribe el contenido del JSON en el archivo
            writer.close(); 

            System.out.printf("Save created in: %s", saveFile.getAbsolutePath()); 
        }
        catch (Exception e)
        {
            System.out.printf("Error: %s", e.getMessage());
        }
    }
}