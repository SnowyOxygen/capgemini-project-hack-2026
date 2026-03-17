import { View, Text, StyleSheet, TouchableOpacity, SafeAreaView, Platform, StatusBar } from 'react-native';
import { useRouter } from 'expo-router';
import { Ionicons } from '@expo/vector-icons';

export default function DashboardScreen() {
  const router = useRouter();

  const handleLogout = () => {
    router.replace('/');
  };

  const handleAddBatiment = () => {
    alert("Add Batiment Action Triggered!");
  };

  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.container}>
        {/* Header acts as Top Navigation */}
        <View style={styles.header}>
          <View style={styles.headerLeft}>
            <Ionicons name="business" size={28} color="#fff" />
            <Text style={styles.headerTitle}>Dashboard</Text>
          </View>
          <TouchableOpacity onPress={handleLogout} style={styles.logoutButton}>
            <Ionicons name="log-out-outline" size={26} color="#fff" />
          </TouchableOpacity>
        </View>

        <View style={styles.content}>
          <Text style={styles.pageTitle}>Dashboard</Text>

          {/* Actions Menu */}
          <View style={styles.actionsContainer}>
            <TouchableOpacity style={styles.actionButton} onPress={handleAddBatiment}>
              <Ionicons name="add-circle-outline" size={24} color="#fff" />
              <Text style={styles.actionText}>Add Site</Text>
            </TouchableOpacity>
          </View>

          {/* KPI Section */}
          <View style={styles.kpiContainer}>
            <View style={styles.kpiCard}>
              <View style={styles.kpiIcon}>
                <Ionicons name="business-outline" size={32} color="#8DC5AA" />
              </View>
              <View>
                <Text style={styles.kpiTitle}>Total Sites</Text>
                <Text style={styles.kpiValue}>No data available</Text>
              </View>
            </View>
          </View>
        </View>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: '#8DC5AA',
    paddingTop: Platform.OS === 'android' ? StatusBar.currentHeight : 0,
  },
  container: {
    flex: 1,
    backgroundColor: '#f4f7f6',
  },
  header: {
    backgroundColor: '#8DC5AA',
    padding: 20,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    borderBottomWidth: 1,
    borderBottomColor: 'rgba(255,255,255,0.1)',
  },
  headerLeft: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 10,
  },
  headerTitle: {
    color: '#fff',
    fontSize: 22,
    fontWeight: 'bold',
  },
  logoutButton: {
    padding: 5,
  },
  content: {
    flex: 1,
    padding: 20,
  },
  pageTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#333',
    marginBottom: 30,
  },
  actionsContainer: {
    marginBottom: 30,
  },
  actionButton: {
    backgroundColor: '#8DC5AA',
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 16,
    borderRadius: 12,
    gap: 10,
  },
  actionText: {
    color: '#fff',
    fontWeight: 'bold',
    fontSize: 16,
  },
  kpiContainer: {
    flex: 1,
  },
  kpiCard: {
    backgroundColor: '#fff',
    padding: 20,
    borderRadius: 16,
    flexDirection: 'row',
    alignItems: 'center',
    gap: 15,
    shadowColor: '#000',
    shadowOpacity: 0.05,
    shadowRadius: 10,
    elevation: 2,
    borderWidth: 1,
    borderColor: '#eaeaea',
  },
  kpiIcon: {
    backgroundColor: 'rgba(141, 197, 170, 0.1)',
    padding: 15,
    borderRadius: 12,
  },
  kpiTitle: {
    fontSize: 14,
    color: '#888',
    marginBottom: 5,
  },
  kpiValue: {
    fontSize: 20,
    fontWeight: 'bold',
    color: '#333',
  },
});
