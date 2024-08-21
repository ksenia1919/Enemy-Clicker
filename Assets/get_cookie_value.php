<?php
$url = "http://ksenia.hostingem.ru/?i=2";
$cookieData = file_get_contents($url);

preg_match_all('/Set-Cookie: ([^;]+)/', $cookieData, $matches);

$cookies = [];
foreach ($matches[1] as $cookie) {
    list($name, $value) = explode('=', $cookie, 2);
    $cookies[$name] = $value;
}

$testCookie = $cookies['__test'] ?? '';

echo {$testCookie};
?>
