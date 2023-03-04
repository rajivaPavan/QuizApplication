function setCountdown(){
    const quizDuration = 1000 * 60 * 30; // 30 minutes
    // Get the starting time from localStorage, or set it to the current time if it's not set yet
    let startTime = localStorage.getItem('quizStartTime') || new Date().getTime();

    // Update the stored starting time in localStorage
    localStorage.setItem('quizStartTime', startTime);

    // Define the function to update the timer display
    function updateTimer() {
        let currentTime = new Date().getTime();
        let elapsedTime = currentTime - startTime;
        // If the quiz duration has been exceeded, remove the stored starting time
        if (elapsedTime > quizDuration) {
            localStorage.removeItem('quizStartTime');
        }
        let secondsRemaining = Math.max(0, Math.floor((quizDuration - elapsedTime) / 1000));
        let minutesRemaining = Math.floor(secondsRemaining / 60);
        let secondsDisplay = secondsRemaining % 60;
        let timerDisplay = minutesRemaining.toString().padStart(2, '0') + ':' + secondsDisplay.toString().padStart(2, '0');
        document.getElementById('timer').textContent = timerDisplay;
    }

    // Call the updateTimer function every second
    setInterval(updateTimer, 1000);
}


// document ready
$(function () {
    $('#submitCheck').hide();
    setCountdown();

});

// submit button
$('#finishBtn').click(function () {
    $('#submitCheck').show();
});
$('#dontFinish').click(function () {
    $('#submitCheck').hide();
});