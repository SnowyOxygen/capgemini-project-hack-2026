import { Text, View } from "react-native";

export default function Index() {
  return (
    <View
      style={{
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
      }}
    >
      <Text style={{ fontSize: 32, fontWeight: 'bold' }}>Hello World!</Text>
      <Text style={{ fontSize: 18, color: '#666', marginTop: 10 }}>This is the mobile app.</Text>
    </View>
  );
}
