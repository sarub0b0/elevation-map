<?php
    $con = mysql_connect('localhost', 'root', '123edsa7896321') or die('error(connect)');

    mysql_select_db('test', $con) or die('error(select_db)');

    $sql = 'select * from sample';
    $result = mysql_query($sql, $con);

    while ($row = mysql_fetch_array($result)) {
        echo $row['id'] . ' : ' . $row['name'] . '<br />';
    }

    mysql_close($con);
?>

