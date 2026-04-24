import java.io.File;
import java.io.FileWriter;

public class SaveSystem
{
    public static void main(String[] args)
    {
        String json;
        File jarFile;
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
            jarFile = new File(SaveSystem.class.getProtectionDomain().getCodeSource().getLocation().toURI()); // Obtiene la ruta del archivo jar.
            folder = new File(jarFile.getParentFile().getParentFile(), "Save"); // Lo que hace es obtener la ruta del padre del archivo jar, para crear la carpeta "Save" en esa ruta.

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