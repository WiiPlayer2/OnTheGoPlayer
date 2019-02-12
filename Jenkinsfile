pipeline {
    agent {
        label 'windows'
    }

    stages {
        stage('Build') {
            steps {
                checkout scm
                powershell './build.ps1 -Target Pack -Configuration Release'
            }
        }

        stage('Archive') {
            steps {
                archiveArtifacts '/*.zip'
                fileOperations([fileDeleteOperation(includes: '/*.zip')])
            }
        }
    }
}
