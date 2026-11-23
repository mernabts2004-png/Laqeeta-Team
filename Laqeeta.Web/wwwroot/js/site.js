// Shows the secret question / answer fields only for high-value categories
// (phone, wallet, ID). The API enforces the same rule on the server.
document.addEventListener('DOMContentLoaded', function () {
    var category = document.getElementById('Category');
    var secretBox = document.getElementById('secretBox');

    if (!category || !secretBox) {
        return;
    }

    function toggleSecretBox() {
        var option = category.options[category.selectedIndex];
        var isHighValue = !!option && option.getAttribute('data-high') === '1';

        secretBox.classList.toggle('d-none', !isHighValue);

        secretBox.querySelectorAll('input').forEach(function (input) {
            input.required = isHighValue;
        });
    }

    category.addEventListener('change', toggleSecretBox);
    toggleSecretBox();
});
