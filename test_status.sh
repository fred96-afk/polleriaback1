curl -X PATCH "https://localhost:7200/api/Pedidos/3/status" \
     -H "accept: */*" \
     -H "Content-Type: application/json" \
     -H "Authorization: Bearer " \
     -d "\"Accepted\"" \
     -k -v
