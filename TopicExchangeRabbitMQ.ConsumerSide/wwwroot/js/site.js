// Function to subscribe to selected groups and start receiving messages
async function receiveMessagesFromSelectedGroups() {
    const checkboxes = document.querySelectorAll("input[type='checkbox']:checked");
    const selectedGroups = Array.from(checkboxes).map(cb => cb.parentElement.textContent.trim());
    console.log(selectedGroups, selectedGroups.length);
    await axios.post('Consumer/SubscribeToGroups', { groupNames: selectedGroups });

    axios.get("Consumer/GetReceivedMessages")
        .then(response => {
            console.log(response);
        });        
       
}


// Function to load subgroups of a selected group
function loadSubgroupsOfSelectedGroup(groupName) {
    const group = document.getElementById(groupName);

    // Check if subgroups are already loaded
    if (group.getAttribute('data-loaded') === 'true') {
        return;
    }

    axios.get(`/Consumer/GetSubGroupsOfGroup?groupName=${groupName}`)
        .then(response => {
            const subgroups = response.data;

            // Avoid duplicates and handle nesting
            subgroups.forEach(subgroup => {
                if (!document.getElementById(subgroup)) {
                    const subgroupElement = document.createElement("div");
                    subgroupElement.id = subgroup;
                    subgroupElement.style = "display:flex;flex-direction:column;cursor:pointer; margin-left:20px;";
                    subgroupElement.onclick = () => loadSubgroupsOfSelectedGroup(subgroup);

                    const label = document.createElement("label");
                    label.innerHTML = `<input type="checkbox"/> ${subgroup}`;
                    subgroupElement.appendChild(label);

                    group.appendChild(subgroupElement);
                }
            });

            // Mark this group as loaded
            group.setAttribute('data-loaded', 'true');
        })
        .catch(error => {
            console.error('Error getting subgroups:', error);
            alert("Failed to load subgroups. Please try again.");
        });
}