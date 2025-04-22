<?php
header('Content-Type: application/json');
echo json_encode([
    'message' => 'Bienvenue sur votre API REST maison. Endpoints disponibles : accounts.php, children.php, invoices.php'
]);
?>
