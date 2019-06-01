pipeline {
    agent {
        label 'windows'
    }

    stages {
        stage('Cleanup') {
            steps {
                fileOperations([fileDeleteOperation(includes: '/_build/*.zip')])
            }
        }

        stage('Build') {
            steps {
                checkout scm
                powershell './build.ps1 -Target Pack'
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/_build/*.zip'
            }
        }
    }
}
