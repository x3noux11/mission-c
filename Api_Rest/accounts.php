<?php
header('Content-Type: application/json');
require_once 'db.php';

$method = $_SERVER['REQUEST_METHOD'];
$id = $_GET['id'] ?? null;

function getInputData() {
    return json_decode(file_get_contents("php://input"), true);
}

switch ($method) {
    case 'GET':
        if ($id) {
            $stmt = $pdo->prepare("SELECT * FROM accounts WHERE id = ?");
            $stmt->execute([$id]);
            $data = $stmt->fetch(PDO::FETCH_ASSOC);
        } else {
            $stmt = $pdo->query("SELECT * FROM accounts");
            $data = $stmt->fetchAll(PDO::FETCH_ASSOC);
        }
        echo json_encode($data);
        break;

    case 'POST':
        $input = getInputData();
        if (isset($input['action']) && $input['action'] === 'register') {
            // Remove the 'action' field from the input array
            unset($input['action']);

            // Insert new account
            if (isset($input['username']) && isset($input['password'])) {
                $columns = implode(", ", array_keys($input));
                $placeholders = implode(", ", array_fill(0, count($input), '?'));
                $stmt = $pdo->prepare("INSERT INTO accounts ($columns) VALUES ($placeholders)");
                $stmt->execute(array_values($input));
                echo json_encode(['success' => true, 'id' => $pdo->lastInsertId()]);
            } else {
                echo json_encode(['success' => false, 'message' => 'Nom d\'utilisateur et mot de passe requis']);
            }
        } else {
            // Login verification
            if (isset($input['username']) && isset($input['password'])) {
                $stmt = $pdo->prepare("SELECT * FROM accounts WHERE username = ?");
                $stmt->execute([$input['username']]);
                $data = $stmt->fetch(PDO::FETCH_ASSOC);
                if ($data && password_verify($input['password'], $data['password'])) {
                    echo json_encode(['success' => true, 'message' => 'Connexion réussie', 'id' => $data['id'], 'typeUser' => $data['typeUser']]);
                } else {
                    echo json_encode(['success' => false, 'message' => 'Nom d\'utilisateur ou mot de passe incorrect']);
                }
            } else {
                echo json_encode(['success' => false, 'message' => 'Nom d\'utilisateur et mot de passe requis']);
            }
        }
        break;

    case 'PUT':
        if (!$id) {
            echo json_encode(['error' => 'ID requis pour la mise à jour']);
            exit;
        }
        $input = getInputData();
        $setClause = implode(", ", array_map(fn($k) => "$k = ?", array_keys($input)));
        $stmt = $pdo->prepare("UPDATE accounts SET $setClause WHERE id = ?");
        $stmt->execute([...array_values($input), $id]);
        echo json_encode(['success' => true]);
        break;

    case 'DELETE':
        if (!$id) {
            echo json_encode(['error' => 'ID requis pour la suppression']);
            exit;
        }
        $stmt = $pdo->prepare("DELETE FROM accounts WHERE id = ?");
        $stmt->execute([$id]);
        echo json_encode(['success' => true]);
        break;

    default:
        http_response_code(405);
        echo json_encode(['error' => 'Méthode non autorisée']);
        break;
}
?>