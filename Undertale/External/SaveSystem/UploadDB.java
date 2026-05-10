import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;

public class UploadDB
{
    static final String DB_NAME = "BD_dadesUndertale";
    static final String DB_SERVER_URL = "jdbc:mysql://localhost:3306/";
    static final String DB_URL = DB_SERVER_URL + DB_NAME;
    static final String DB_USER = "root";
    static final String DB_PASSWORD = "";

    static final String SAVE_FILE = "..\\Save\\save.json";
    static final String LOAD_FILE = "..\\Save\\load.json";

    static final int WAIT_TIME = 1500;
    static final int READ_WAIT_TIME = 300;
    static final int MAX_FIGHTS = 100;
    static final int PLAYER_ID = 1;

    static final double DEFAULT_HEALTH = 20.0;
    static final double DEFAULT_SCORE = 0.0;
    static final String DEFAULT_PLAYER_NAME = "";

    static double health;
    static double score;
    static String playerName;
    static String[] completedFights;

    public static void main(String[] args)
    {
        String option;

        option = "";

        if (args.length == 0)
        {
            System.out.printf("No hay ninguna opcion\n");
            System.out.printf("Opciones: save, load, delete o watch\n");
        }
        else
        {
            option = args[0];

            if (option.equals("save"))
            {
                ensureJsonFiles();
                saveGame();
            }
            else if (option.equals("load"))
            {
                ensureJsonFiles();
                loadGame();
            }
            else if (option.equals("delete"))
            {
                ensureJsonFiles();
                deleteGame();
            }
            else if (option.equals("watch"))
            {
                ensureJsonFiles();
                watchSaveFile();
            }
            else
            {
                System.out.printf("Opcion incorrecta\n");
                System.out.printf("Opciones: save, load, delete o watch\n");
            }
        }
    }

    /**
     * Vigila save.json y guarda automaticamente cuando Unity modifica el archivo.
     */
    public static void watchSaveFile()
    {
        File saveFile;
        long lastModified;
        long currentModified;
        boolean running;

        saveFile = new File(getSaveFilePath());
        lastModified = 0;
        currentModified = 0;
        running = true;

        if (saveFile.exists())
        {
            lastModified = saveFile.lastModified();
        }

        System.out.printf("Modo watch activado\n");
        System.out.printf("Archivo vigilado: %s\n", saveFile.getAbsolutePath());

        while (running)
        {
            try
            {
                ensureJsonFiles();
                saveFile = new File(getSaveFilePath());

                if (saveFile.exists())
                {
                    currentModified = saveFile.lastModified();

                    if (currentModified != lastModified)
                    {
                        lastModified = currentModified;
                        System.out.printf("Cambio detectado en save.json\n");
                        Thread.sleep(READ_WAIT_TIME);
                        saveGame();
                    }
                }

                Thread.sleep(WAIT_TIME);
            }
            catch (Exception e)
            {
                System.out.printf("Error en watch\n");
                System.out.printf("%s\n", e.getMessage());
            }
        }
    }

    /**
     * Crea la base de datos y las tablas si todavia no existen.
     */
    public static boolean prepareDatabase()
    {
        Connection connection;
        PreparedStatement preparedStatement;
        String sql;
        boolean prepared;

        connection = null;
        preparedStatement = null;
        sql = "";
        prepared = false;

        try
        {
            Class.forName("com.mysql.cj.jdbc.Driver");
            connection = DriverManager.getConnection(DB_SERVER_URL, DB_USER, DB_PASSWORD);

            sql = "CREATE DATABASE IF NOT EXISTS " + DB_NAME;
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.executeUpdate();
            preparedStatement.close();

            connection.close();

            connection = connectDB();

            if (connection != null)
            {
                sql = "CREATE TABLE IF NOT EXISTS players ("
                    + "id INT AUTO_INCREMENT PRIMARY KEY, "
                    + "player_name VARCHAR(100) NOT NULL UNIQUE, "
                    + "health DOUBLE NOT NULL, "
                    + "score DOUBLE NOT NULL"
                    + ")";
                preparedStatement = connection.prepareStatement(sql);
                preparedStatement.executeUpdate();
                preparedStatement.close();

                sql = "CREATE TABLE IF NOT EXISTS completed_fights ("
                    + "id INT AUTO_INCREMENT PRIMARY KEY, "
                    + "player_id INT NOT NULL, "
                    + "fight_name VARCHAR(100) NOT NULL, "
                    + "FOREIGN KEY (player_id) REFERENCES players(id) ON DELETE CASCADE"
                    + ")";
                preparedStatement = connection.prepareStatement(sql);
                preparedStatement.executeUpdate();
                preparedStatement.close();

                connection.close();
                prepared = true;
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al preparar la Base de Datos\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return prepared;
    }

    /**
     * Abre la conexion con MySQL.
     */
    public static Connection connectDB()
    {
        Connection connection;

        connection = null;

        try
        {
            Class.forName("com.mysql.cj.jdbc.Driver");
            connection = DriverManager.getConnection(DB_URL, DB_USER, DB_PASSWORD);
        }
        catch (Exception e)
        {
            System.out.printf("Error al conectar con la Base de Datos\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return connection;
    }

    /**
     * Devuelve la ruta de save.json aunque Java se lance desde otra carpeta.
     */
    public static String getSaveFilePath()
    {
        File file;
        String path;

        path = SAVE_FILE;
        file = new File(path);

        if (!file.exists())
        {
            path = getProgramFolder() + "\\..\\Save\\save.json";
        }

        return path;
    }

    /**
     * Devuelve la ruta de la carpeta donde estan save.json y load.json.
     */
    public static String getSaveFolderPath()
    {
        File folder;
        String path;

        path = "..\\Save";
        folder = new File(path);

        if (!folder.exists())
        {
            path = getProgramFolder() + "\\..\\Save";
        }

        return path;
    }

    /**
     * Devuelve la ruta de load.json aunque Java se lance desde otra carpeta.
     */
    public static String getLoadFilePath()
    {
        File folder;
        String path;

        path = LOAD_FILE;
        folder = new File("..\\Save");

        if (!folder.exists())
        {
            path = getProgramFolder() + "\\..\\Save\\load.json";
        }

        return path;
    }

    /**
     * Crea la carpeta Save y los JSON si alguien los ha borrado.
     */
    public static void ensureJsonFiles()
    {
        File folder;
        File saveFile;
        File loadFile;

        folder = new File(getSaveFolderPath());
        saveFile = null;
        loadFile = null;

        try
        {
            if (!folder.exists())
            {
                folder.mkdirs();
            }

            saveFile = new File(getSaveFilePath());
            loadFile = new File(getLoadFilePath());

            if (!saveFile.exists())
            {
                createDefaultSaveFile();
            }

            if (!loadFile.exists())
            {
                createDefaultLoadFile();
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al comprobar los archivos JSON\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Rellena las variables con una partida vacia.
     */
    public static void setDefaultData()
    {
        health = DEFAULT_HEALTH;
        score = DEFAULT_SCORE;
        playerName = DEFAULT_PLAYER_NAME;
        completedFights = new String[0];
    }

    /**
     * Crea save.json con valores por defecto.
     */
    public static void createDefaultSaveFile()
    {
        setDefaultData();
        writeJsonFile(getSaveFilePath());
        System.out.printf("save.json no existia y se ha creado automaticamente\n");
    }

    /**
     * Crea load.json con valores por defecto.
     */
    public static void createDefaultLoadFile()
    {
        setDefaultData();
        writeJsonFile(getLoadFilePath());
        System.out.printf("load.json no existia y se ha creado automaticamente\n");
    }

    /**
     * Busca la carpeta donde esta UploadDB.class.
     */
    public static String getProgramFolder()
    {
        File programFile;
        String folder;

        programFile = null;
        folder = ".";

        try
        {
            programFile = new File(UploadDB.class.getProtectionDomain().getCodeSource().getLocation().toURI());

            if (programFile.isFile())
            {
                folder = programFile.getParent();
            }
            else
            {
                folder = programFile.getPath();
            }
        }
        catch (Exception e)
        {
            System.out.printf("No se ha podido detectar la carpeta del programa\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return folder;
    }

    /**
     * Lee save.json y guarda la partida en MySQL.
     */
    public static void saveGame()
    {
        Connection connection;
        boolean fileRead;
        boolean databaseReady;
        int playerId;

        connection = null;
        fileRead = false;
        databaseReady = false;
        playerId = 0;

        try
        {
            fileRead = readSaveFile();

            if (fileRead)
            {
                databaseReady = prepareDatabase();

                if (databaseReady)
                {
                    connection = connectDB();

                    if (connection != null)
                    {
                        playerId = PLAYER_ID;
                        deleteOtherPlayers(connection);

                        if (playerExists(connection, playerId))
                        {
                            updatePlayer(connection, playerId);
                        }
                        else
                        {
                            insertPlayer(connection);
                        }

                        deleteCompletedFights(connection, playerId);
                        insertCompletedFights(connection, playerId);
                        createLoadFile();
                        System.out.printf("Partida guardada en MySQL\n");

                        connection.close();
                    }
                }
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al guardar el juego\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Recupera la partida de MySQL y crea load.json para Unity.
     */
    public static void loadGame()
    {
        Connection connection;
        PreparedStatement preparedStatement;
        ResultSet resultSet;
        String sql;
        boolean databaseReady;
        int playerId;

        connection = null;
        preparedStatement = null;
        resultSet = null;
        sql = "";
        databaseReady = false;
        playerId = 0;

        try
        {
            databaseReady = prepareDatabase();

            if (databaseReady)
            {
                connection = connectDB();

                if (connection != null)
                {
                    sql = "SELECT id, player_name, health, score FROM players WHERE id = ?";
                    preparedStatement = connection.prepareStatement(sql);
                    preparedStatement.setInt(1, PLAYER_ID);
                    resultSet = preparedStatement.executeQuery();

                    if (resultSet.next())
                    {
                        playerId = resultSet.getInt("id");
                        playerName = resultSet.getString("player_name");
                        health = resultSet.getDouble("health");
                        score = resultSet.getDouble("score");

                        loadCompletedFights(connection, playerId);
                        createLoadFile();

                        System.out.printf("Archivo load.json creado\n");
                    }
                    else
                    {
                        System.out.printf("No hay partidas guardadas en MySQL\n");
                        setDefaultData();
                        createLoadFile();
                    }

                    resultSet.close();
                    preparedStatement.close();
                    connection.close();
                }
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al cargar el juego\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Borra de MySQL el jugador indicado en save.json.
     */
    public static void deleteGame()
    {
        Connection connection;
        PreparedStatement preparedStatement;
        String sql;
        boolean fileRead;
        boolean databaseReady;
        int playerId;

        connection = null;
        preparedStatement = null;
        sql = "";
        fileRead = false;
        databaseReady = false;
        playerId = 0;

        try
        {
            fileRead = readSaveFile();

            if (fileRead)
            {
                databaseReady = prepareDatabase();

                if (databaseReady)
                {
                    connection = connectDB();

                    if (connection != null)
                    {
                        playerId = PLAYER_ID;
                        deleteOtherPlayers(connection);
                        deleteCompletedFights(connection, playerId);

                        sql = "DELETE FROM players WHERE id = ?";
                        preparedStatement = connection.prepareStatement(sql);
                        preparedStatement.setInt(1, playerId);
                        preparedStatement.executeUpdate();

                        preparedStatement.close();
                        connection.close();

                        setDefaultData();
                        createLoadFile();
                        System.out.printf("Jugador eliminado de MySQL\n");
                    }
                }
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al eliminar la partida\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Lee todo el contenido de save.json y rellena las variables del jugador.
     */
    public static boolean readSaveFile()
    {
        File saveFile;
        BufferedReader reader;
        String line;
        String json;
        boolean fileRead;

        saveFile = null;
        reader = null;
        line = "";
        json = "";
        fileRead = false;

        try
        {
            saveFile = new File(getSaveFilePath());

            if (!saveFile.exists())
            {
                createDefaultSaveFile();
            }

            reader = new BufferedReader(new FileReader(getSaveFilePath()));
            line = reader.readLine();

            while (line != null)
            {
                json = json + line;
                line = reader.readLine();
            }

            reader.close();

            health = getDoubleValue(json, "health");
            score = getDoubleValue(json, "score");
            playerName = getStringValue(json, "playerName");
            completedFights = getArrayValue(json, "completedFights");

            fileRead = true;
        }
        catch (Exception e)
        {
            System.out.printf("Error al leer save.json\n");
            System.out.printf("Ruta usada: %s\n", getSaveFilePath());
            System.out.printf("%s\n", e.getMessage());
        }

        return fileRead;
    }

    /**
     * Busca el id de un jugador por su nombre.
     */
    public static int findPlayerId(Connection connection, String name)
    {
        PreparedStatement preparedStatement;
        ResultSet resultSet;
        String sql;
        int playerId;

        preparedStatement = null;
        resultSet = null;
        sql = "SELECT id FROM players WHERE player_name = ?";
        playerId = 0;

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setString(1, name);
            resultSet = preparedStatement.executeQuery();

            if (resultSet.next())
            {
                playerId = resultSet.getInt("id");
            }

            resultSet.close();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al buscar el jugador\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return playerId;
    }

    /**
     * Comprueba si existe la ranura fija de guardado.
     */
    public static boolean playerExists(Connection connection, int playerId)
    {
        PreparedStatement preparedStatement;
        ResultSet resultSet;
        String sql;
        boolean exists;

        preparedStatement = null;
        resultSet = null;
        sql = "SELECT id FROM players WHERE id = ?";
        exists = false;

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, playerId);
            resultSet = preparedStatement.executeQuery();

            if (resultSet.next())
            {
                exists = true;
            }

            resultSet.close();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al comprobar si existe el jugador\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return exists;
    }

    /**
     * Calcula el siguiente id disponible de una tabla.
     */
    public static int getNextId(Connection connection, String tableName)
    {
        PreparedStatement preparedStatement;
        ResultSet resultSet;
        String sql;
        int nextId;

        preparedStatement = null;
        resultSet = null;
        sql = "SELECT MAX(id) AS max_id FROM " + tableName;
        nextId = 1;

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            resultSet = preparedStatement.executeQuery();

            if (resultSet.next())
            {
                nextId = resultSet.getInt("max_id") + 1;
            }

            resultSet.close();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al calcular el siguiente id\n");
            System.out.printf("%s\n", e.getMessage());
        }

        return nextId;
    }

    /**
     * Inserta un jugador nuevo en la tabla players.
     */
    public static void insertPlayer(Connection connection)
    {
        PreparedStatement preparedStatement;
        String sql;

        preparedStatement = null;
        sql = "INSERT INTO players (id, player_name, health, score) VALUES (?, ?, ?, ?)";

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, PLAYER_ID);
            preparedStatement.setString(2, playerName);
            preparedStatement.setDouble(3, health);
            preparedStatement.setDouble(4, score);
            preparedStatement.executeUpdate();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al insertar el jugador\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Actualiza los datos principales del jugador.
     */
    public static void updatePlayer(Connection connection, int playerId)
    {
        PreparedStatement preparedStatement;
        String sql;

        preparedStatement = null;
        sql = "UPDATE players SET player_name = ?, health = ?, score = ? WHERE id = ?";

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setString(1, playerName);
            preparedStatement.setDouble(2, health);
            preparedStatement.setDouble(3, score);
            preparedStatement.setInt(4, playerId);
            preparedStatement.executeUpdate();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al actualizar el jugador\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Limpia jugadores antiguos creados durante pruebas y deja solo la ranura 1.
     */
    public static void deleteOtherPlayers(Connection connection)
    {
        PreparedStatement preparedStatement;
        String sql;

        preparedStatement = null;
        sql = "";

        try
        {
            sql = "DELETE FROM completed_fights WHERE player_id <> ?";
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, PLAYER_ID);
            preparedStatement.executeUpdate();
            preparedStatement.close();

            sql = "DELETE FROM players WHERE id <> ?";
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, PLAYER_ID);
            preparedStatement.executeUpdate();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al limpiar jugadores antiguos\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Borra los combates anteriores del jugador antes de guardar los nuevos.
     */
    public static void deleteCompletedFights(Connection connection, int playerId)
    {
        PreparedStatement preparedStatement;
        String sql;

        preparedStatement = null;
        sql = "DELETE FROM completed_fights WHERE player_id = ?";

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, playerId);
            preparedStatement.executeUpdate();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al borrar combates anteriores\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Inserta en MySQL los combates completados del JSON.
     */
    public static void insertCompletedFights(Connection connection, int playerId)
    {
        PreparedStatement preparedStatement;
        String sql;
        int i;

        preparedStatement = null;
        sql = "INSERT INTO completed_fights (id, player_id, fight_name) VALUES (?, ?, ?)";
        i = 0;

        try
        {
            if (completedFights != null)
            {
                preparedStatement = connection.prepareStatement(sql);

                for (i = 0; i < completedFights.length; i++)
                {
                    preparedStatement.setInt(1, getNextId(connection, "completed_fights"));
                    preparedStatement.setInt(2, playerId);
                    preparedStatement.setString(3, completedFights[i]);
                    preparedStatement.executeUpdate();
                }

                preparedStatement.close();
            }
        }
        catch (Exception e)
        {
            System.out.printf("Error al insertar combates\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Carga desde MySQL los combates completados del jugador.
     */
    public static void loadCompletedFights(Connection connection, int playerId)
    {
        PreparedStatement preparedStatement;
        ResultSet resultSet;
        String sql;
        String[] fights;
        int count;

        preparedStatement = null;
        resultSet = null;
        sql = "SELECT fight_name FROM completed_fights WHERE player_id = ? ORDER BY id";
        fights = new String[MAX_FIGHTS];
        count = 0;

        try
        {
            preparedStatement = connection.prepareStatement(sql);
            preparedStatement.setInt(1, playerId);
            resultSet = preparedStatement.executeQuery();

            while (resultSet.next() && count < MAX_FIGHTS)
            {
                fights[count] = resultSet.getString("fight_name");
                count++;
            }

            completedFights = copyFights(fights, count);

            resultSet.close();
            preparedStatement.close();
        }
        catch (Exception e)
        {
            completedFights = new String[0];
            System.out.printf("Error al cargar combates\n");
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Crea load.json con la misma estructura que save.json.
     */
    public static void createLoadFile()
    {
        writeJsonFile(getLoadFilePath());
    }

    /**
     * Escribe las variables actuales en un archivo JSON.
     */
    public static void writeJsonFile(String filePath)
    {
        FileWriter writer;
        int i;

        writer = null;
        i = 0;

        try
        {
            writer = new FileWriter(filePath);
            writer.write("{\n");
            writer.write("    \"health\": " + health + ",\n");
            writer.write("    \"score\": " + score + ",\n");
            writer.write("    \"playerName\": \"" + escapeJson(playerName) + "\",\n");
            writer.write("    \"completedFights\": [\n");

            if (completedFights != null)
            {
                for (i = 0; i < completedFights.length; i++)
                {
                    writer.write("        \"" + escapeJson(completedFights[i]) + "\"");

                    if (i < completedFights.length - 1)
                    {
                        writer.write(",");
                    }

                    writer.write("\n");
                }
            }

            writer.write("    ]\n");
            writer.write("}\n");
            writer.close();
        }
        catch (Exception e)
        {
            System.out.printf("Error al crear archivo JSON\n");
            System.out.printf("Ruta usada: %s\n", filePath);
            System.out.printf("%s\n", e.getMessage());
        }
    }

    /**
     * Lee un numero decimal de una clave del JSON.
     */
    public static double getDoubleValue(String json, String key)
    {
        int keyPosition;
        int colonPosition;
        int commaPosition;
        int bracePosition;
        int endPosition;
        String value;
        double number;

        keyPosition = json.indexOf("\"" + key + "\"");
        colonPosition = json.indexOf(":", keyPosition);
        commaPosition = json.indexOf(",", colonPosition);
        bracePosition = json.indexOf("}", colonPosition);
        endPosition = commaPosition;
        value = "";
        number = 0;

        if (endPosition == -1 || (bracePosition != -1 && bracePosition < endPosition))
        {
            endPosition = bracePosition;
        }

        value = json.substring(colonPosition + 1, endPosition);
        value = value.trim();
        number = Double.parseDouble(value);

        return number;
    }

    /**
     * Lee un texto de una clave del JSON.
     */
    public static String getStringValue(String json, String key)
    {
        int keyPosition;
        int colonPosition;
        int firstQuotePosition;
        int secondQuotePosition;
        String value;

        keyPosition = json.indexOf("\"" + key + "\"");
        colonPosition = json.indexOf(":", keyPosition);
        firstQuotePosition = json.indexOf("\"", colonPosition + 1);
        secondQuotePosition = json.indexOf("\"", firstQuotePosition + 1);
        value = json.substring(firstQuotePosition + 1, secondQuotePosition);
        value = unescapeJson(value);

        return value;
    }

    /**
     * Lee un array sencillo de textos del JSON.
     */
    public static String[] getArrayValue(String json, String key)
    {
        int keyPosition;
        int startPosition;
        int endPosition;
        String arrayText;
        String[] values;
        int count;
        int position;
        int firstQuotePosition;
        int secondQuotePosition;

        keyPosition = json.indexOf("\"" + key + "\"");
        startPosition = json.indexOf("[", keyPosition);
        endPosition = json.indexOf("]", startPosition);
        arrayText = "";
        values = new String[MAX_FIGHTS];
        count = 0;
        position = 0;
        firstQuotePosition = 0;
        secondQuotePosition = 0;

        if (startPosition != -1 && endPosition != -1)
        {
            arrayText = json.substring(startPosition + 1, endPosition);
            position = 0;

            while (position < arrayText.length() && count < MAX_FIGHTS)
            {
                firstQuotePosition = arrayText.indexOf("\"", position);

                if (firstQuotePosition != -1)
                {
                    secondQuotePosition = arrayText.indexOf("\"", firstQuotePosition + 1);

                    if (secondQuotePosition != -1)
                    {
                        values[count] = arrayText.substring(firstQuotePosition + 1, secondQuotePosition);
                        values[count] = unescapeJson(values[count]);
                        count++;
                        position = secondQuotePosition + 1;
                    }
                    else
                    {
                        position = arrayText.length();
                    }
                }
                else
                {
                    position = arrayText.length();
                }
            }
        }

        values = copyFights(values, count);

        return values;
    }

    /**
     * Copia solo las posiciones usadas del array de combates.
     */
    public static String[] copyFights(String[] fights, int count)
    {
        String[] copy;
        int i;

        copy = new String[count];
        i = 0;

        for (i = 0; i < count; i++)
        {
            copy[i] = fights[i];
        }

        return copy;
    }

    /**
     * Prepara un texto para escribirlo dentro del JSON.
     */
    public static String escapeJson(String text)
    {
        String value;

        value = "";

        if (text != null)
        {
            value = text.replace("\\", "\\\\");
            value = value.replace("\"", "\\\"");
        }

        return value;
    }

    /**
     * Limpia escapes sencillos leidos desde el JSON.
     */
    public static String unescapeJson(String text)
    {
        String value;

        value = "";

        if (text != null)
        {
            value = text.replace("\\\"", "\"");
            value = value.replace("\\\\", "\\");
        }

        return value;
    }
}
