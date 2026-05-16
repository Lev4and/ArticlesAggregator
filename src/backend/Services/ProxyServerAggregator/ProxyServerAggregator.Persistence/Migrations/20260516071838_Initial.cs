using System;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProxyServerAggregator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<JsonDocument>(type: "jsonb", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    worker_id = table.Column<string>(type: "text", nullable: true),
                    attempt_deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    attempts_remaining = table.Column<int>(type: "integer", nullable: true),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    entity_state = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proxy_servers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    normalized_name = table.Column<string>(type: "text", nullable: false),
                    protocol = table.Column<string>(type: "text", nullable: false),
                    hostname_or_address = table.Column<string>(type: "text", nullable: false),
                    port = table.Column<int>(type: "integer", nullable: false),
                    credentials_username = table.Column<string>(type: "text", nullable: true),
                    credentials_password = table.Column<string>(type: "text", nullable: true),
                    entity_state = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proxy_servers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "proxy_server_test_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    proxy_server_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    request_time = table.Column<long>(type: "bigint", nullable: true),
                    response_time = table.Column<long>(type: "bigint", nullable: true),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    entity_state = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_proxy_server_test_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_proxy_server_test_requests_proxy_servers_proxy_server_id",
                        column: x => x.proxy_server_id,
                        principalTable: "proxy_servers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_attempt_deadline",
                table: "outbox_messages",
                column: "attempt_deadline");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_attempts",
                table: "outbox_messages",
                column: "attempts");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_attempts_remaining",
                table: "outbox_messages",
                column: "attempts_remaining");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_created_at",
                table: "outbox_messages",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_deadline",
                table: "outbox_messages",
                column: "deadline");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_entity_state",
                table: "outbox_messages",
                column: "entity_state");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_state",
                table: "outbox_messages",
                column: "state");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_state_attempt_deadline_attempts_remaining_d",
                table: "outbox_messages",
                columns: new[] { "state", "attempt_deadline", "attempts_remaining", "deadline" });

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_type",
                table: "outbox_messages",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_updated_at",
                table: "outbox_messages",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_worker_id",
                table: "outbox_messages",
                column: "worker_id");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_created_at",
                table: "proxy_server_test_requests",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_deleted_at",
                table: "proxy_server_test_requests",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_entity_state",
                table: "proxy_server_test_requests",
                column: "entity_state");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_is_deleted",
                table: "proxy_server_test_requests",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_proxy_server_id",
                table: "proxy_server_test_requests",
                column: "proxy_server_id");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_request_time",
                table: "proxy_server_test_requests",
                column: "request_time");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_response_time",
                table: "proxy_server_test_requests",
                column: "response_time");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_status",
                table: "proxy_server_test_requests",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_server_test_requests_updated_at",
                table: "proxy_server_test_requests",
                column: "updated_at");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_created_at",
                table: "proxy_servers",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_credentials_password",
                table: "proxy_servers",
                column: "credentials_password");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_credentials_username",
                table: "proxy_servers",
                column: "credentials_username");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_deleted_at",
                table: "proxy_servers",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_entity_state",
                table: "proxy_servers",
                column: "entity_state");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_hostname_or_address",
                table: "proxy_servers",
                column: "hostname_or_address");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_is_deleted",
                table: "proxy_servers",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_normalized_name",
                table: "proxy_servers",
                column: "normalized_name");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_port",
                table: "proxy_servers",
                column: "port");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_protocol",
                table: "proxy_servers",
                column: "protocol");

            migrationBuilder.CreateIndex(
                name: "ix_proxy_servers_updated_at",
                table: "proxy_servers",
                column: "updated_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "proxy_server_test_requests");

            migrationBuilder.DropTable(
                name: "proxy_servers");
        }
    }
}
