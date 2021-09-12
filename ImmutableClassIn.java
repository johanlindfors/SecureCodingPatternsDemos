final class Student {
    private final String name;
    private final int regNo;
    private final Map<String, String> metadata;

    public Student(String name, int regNo,
                   Map<String, String> metadata) {
        this.name = name;
        this.regNo = regNo;
        this.metadata = cloneMap(metadata);
    }

    public String getName() { return name; }

    public int getRegNo() { return regNo; }

    public Map<String, String> getMetadata() {
        return cloneMap(this.metadata);
    }

    private Map<String, String> cloneMap(
        Map<String, String metadata) {
        Map<String, String> tempMap = new HashMap<>();

        for (Map.Entry<String, String> entry :
            metadata.entrySet()) {
            tempMap.put(entry.getKey(), entry.getValue());
        }
        return tempMap;
    }
}