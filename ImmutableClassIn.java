import java.util.Map;
import java.util.HashMap;

final class Student {
    private final String name;
    private final int regNo;
    private final Map<String, String> metadata;

    public Student(String name, int regNo,
                   Map<String, String> metadata) {
        this.name = name;
        this.regNo = regNo;
        this.metadata = this.cloneMap(metadata);
    }

    public String getName() { return name; }

    public int getRegNo() { return regNo; }

    public Map<String, String> getMetadata() {
        return this.cloneMap(this.metadata);
    }

    private Map<String, String> cloneMap(
        Map<String, String> metadata) {
        Map<String, String> tempMap = new HashMap<>();

        for (Map.Entry<String, String> entry :
            metadata.entrySet()) {
            tempMap.put(entry.getKey(), entry.getValue());
        }
        return tempMap;
    }

    public static void main(String[] args) {
        Map<String,String> metadata = new HashMap<>();
        metadata.put("City", "Vallentuna");
        metadata.put("Company", "Truesec");

        Student student = new Student("Johan", 1234, metadata);

        metadata.remove("Company");

        String company = student.getMetadata().get("Company");
        System.out.println(company);
    }
}
