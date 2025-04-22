<?php
header('Content-Type: application/json');
require_once 'db.php';

$method = $_SERVER['REQUEST_METHOD'];
$id = $_GET['id'] ?? null;
$accountID = $_GET['accountID'] ?? null;

function getInputData() {
    return json_decode(file_get_contents("php://input"), true);
}

switch ($method) {
    case 'GET':
        if ($accountID) {
            $stmt = $pdo->prepare("SELECT * FROM invoices WHERE parent_id = ?");
            $stmt->execute([$accountID]);
            $data = $stmt->fetchAll(PDO::FETCH_ASSOC);
        } elseif ($id) {
            $stmt = $pdo->prepare("SELECT * FROM invoices WHERE id = ?");
            $stmt->execute([$id]);
            $data = $stmt->fetch(PDO::FETCH_ASSOC);
        } else {
            $stmt = $pdo->query("SELECT * FROM invoices");
            $data = $stmt->fetchAll(PDO::FETCH_ASSOC);
        }
        echo json_encode($data);
        break;

    case 'POST':
        $input = getInputData();
        $columns = implode(", ", array_keys($input));
        $placeholders = implode(", ", array_fill(0, count($input), '?'));
        $stmt = $pdo->prepare("INSERT INTO invoices ($columns) VALUES ($placeholders)");
        $stmt->execute(array_values($input));
        echo json_encode(['success' => true, 'id' => $pdo->lastInsertId()]);
        break;

    case 'PUT':
        if (!$id) {
            echo json_encode(['error' => 'ID requis pour la mise à jour']);
            exit;
        }
        $input = getInputData();
        $setClause = implode(", ", array_map(fn($k) => "$k = ?", array_keys($input)));
        $stmt = $pdo->prepare("UPDATE invoices SET $setClause WHERE id = ?");
        $stmt->execute([...array_values($input), $id]);
        echo json_encode(['success' => true]);
        break;

    case 'DELETE':
        if (!$id) {
            echo json_encode(['error' => 'ID requis pour la suppression']);
            exit;
        }
        $stmt = $pdo->prepare("DELETE FROM invoices WHERE id = ?");
        $stmt->execute([$id]);
        echo json_encode(['success' => true]);
        break;

    default:
        http_response_code(405);
        echo json_encode(['error' => 'Méthode non autorisée']);
        break;
}
?>